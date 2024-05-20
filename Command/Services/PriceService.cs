using Command.Context;
using Command.Models;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using Skender.Stock.Indicators;

namespace Command.Services
{
    public class PriceService : Price.PriceBase
    {
        private readonly ILogger<PriceService> logger;
        private readonly CommandContext context;
        private readonly IMemoryCache _cahce;

        public PriceService(ILogger<PriceService> logger, CommandContext context, IMemoryCache cahce)
        {
            this.logger = logger;
            this.context = context;
            _cahce = cahce;
        }

        public List<Models.Price> GetQuetes(string symbol)
        {
            List<Models.Price> quotes = new List<Models.Price>();
            using (StreamReader sr = new StreamReader("./" + symbol + ".csv"))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var split = line.Split("|");
                    Models.Price q = new Models.Price
                    {
                        Symbol = symbol,
                        Time = DateTime.Parse(split[0]),
                        Open = double.Parse(split[1]),
                        High = double.Parse(split[2]),
                        Low = double.Parse(split[3]),
                        Close = double.Parse(split[4]),
                        TickVolume= double.Parse(split[6])
                    };
                    quotes.Add(q);
                }
            }
            return quotes;
        }

        public override async Task GetPrice(PriceRequest request, IServerStreamWriter<PriceData> responseStream, ServerCallContext context)
        {
            DateTime startTime = new DateTime(request.Start);
            DateTime endTime = new DateTime(request.End);
            var prices = _cahce.GetOrCreate($"{request.Symbol}-{request.Start}->{request.End}", (e) => {
                return this.context.Prices.Where(x => x.Symbol == request.Symbol).ToList().Where(x => x.Time > startTime && x.Time < endTime).Select(x => x.ToRPC()).OrderBy(x => x.Time).ToList();
            })!;
            await Task.Delay(5);
            foreach (var item in prices)
            {
                await responseStream.WriteAsync(item);
            }
        }

        /*public override async Task<PriceData> GetPrice(PriceRequest request, ServerCallContext context)
        {
            var startDate = new DateTime(request.Start);
            var endDate = new DateTime(request.End);
            await Task.Delay(TimeSpan.FromMinutes(1));
            return new PriceData { Close = 0, High = 0, Low = 0, Open = 0, Symbol = request.Symbol, Time = request.Start };
        }*/
    }
}

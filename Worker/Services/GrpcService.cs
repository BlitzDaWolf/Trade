using Grpc.Net.Client;
using Grpc.Core;
using Command;
using MassTransit.Internals.Caching;
using Skender.Stock.Indicators;

namespace Worker.Services
{
    public class GrpcService
    {
        private readonly ILogger<GrpcService> _logger;
        private readonly Grpc.Net.Client.GrpcChannel _channel;

        public GrpcService(ILogger<GrpcService> logger)
        {
            _logger = logger;
            _channel = GrpcChannel.ForAddress("http://localhost:5000");
        }

        public async Task<List<Quote>> GetPrice(string symbol, DateTime start, DateTime end)
        {
            var client = new Price.PriceClient(_channel);
            AsyncServerStreamingCall<PriceData> stream = client.GetPrice(new PriceRequest { Symbol = symbol, End = end.Ticks, Start = start.Ticks });

            List<Quote> prices = new List<Quote>();

            await foreach(var response in stream.ResponseStream.ReadAllAsync())
            {
                prices.Add(new Quote
                {
                    Close = (decimal)response.Close,
                    Open = (decimal)response.Open,
                    High = (decimal)response.High,
                    Low = (decimal)response.Low,
                    Date = new DateTime(response.Time)
                });
            }

            return prices.OrderBy(x => x.Date).ToList();

            // await client.GetPriceAsync(new PriceRequest { Symbol = "EURUSD", End = now.Ticks, Start = now.AddDays(-7).Ticks });
        }
    }
}

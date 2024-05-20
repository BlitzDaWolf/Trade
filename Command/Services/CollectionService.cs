using Command.Context;
using Grpc.Core;
using MassTransit;
using Shared;
using Shared.Backtest;
using Skender.Stock.Indicators;
using static System.Net.Mime.MediaTypeNames;

namespace Command.Services
{
    public class CollectionService : Collection.CollectionBase
    {
        private readonly ILogger<CollectionService> _logger;
        private readonly CommandContext _context;
        private Dictionary<string, Type> types;

        public CollectionService(ILogger<CollectionService> logger, CommandContext context)
        {
            _logger = logger;
            _context = context;

            {
                var type = typeof(BaseSignal);
                types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p))
                    .ToDictionary(x => x.Name);
            }
        }

        public override async Task<Response> GetPrice(CollectionPrice request, ServerCallContext context)
        {
            DateTime lastDay = new DateTime(request.Time);
            lastDay = new DateTime(lastDay.Year, lastDay.Month, lastDay.Day);
            lastDay = lastDay.AddDays(-1);

            CollectionPrice curr = request;
            var price = Command.Models.Price.FromRpc(curr);
            if (!_context.Prices.Where(x => x.Symbol == price.Symbol).Select(x => x.Time).Contains(price.Time))
            {
                _context.Prices.Add(price);
                await _context.SaveChangesAsync();
            }

            var result = _context.Results.Where(x => x.Symbol == price.Symbol).ToList().Where(x => x.End == lastDay).MaxBy(x => x.PnL);
            if (result != null)
            {
                var arguments = _context.StrategyArguments.Where(x => x.ResultModelId == result.Id);
                var grabAmount = arguments.Max(x => x.Value);
                // Recive quetes
                var prices = this._context.Prices.Where(x => x.Symbol == request.Symbol).ToList().Where(x => x.Time > lastDay.AddDays(-7) && x.Time < lastDay).Select(x => x.ToQuote()).OrderBy(x => x.Date).ToList();
                BackTest bt = new BackTest(prices);
                foreach (var item in arguments)
                {
                    bt.AddSignal(types[item.Strategy]);
                }
                bt.SetArguments(arguments.Select(x => new StrategyArgument(x.Strategy, x.Argument, x.Value)).ToArray());
                var signal = bt.GetLastSignal();
                _logger.LogInformation("{symbol}: {signal}", request.Symbol, signal);
                return new Response { Signal = signal };
            }
            return new Response { Signal = 0 };
        }
    }
}

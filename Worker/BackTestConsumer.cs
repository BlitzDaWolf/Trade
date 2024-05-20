using MassTransit;
using MassTransit.NewIdProviders;
using Shared;
using Shared.Backtest;
using Skender.Stock.Indicators;
using System.Diagnostics;
using Worker.Services;

namespace Worker
{
    public struct TestResult
    {
        public int TotalTrades => WinTrades + LossTrades;
        public int WinTrades { get; set; }
        public int LossTrades { get; set; }
        public int ConsequentWins { get; set; }
        public int ConsequentLosses { get; set; }
        public double Profit { get; set; }

        public double Score { get; set; }
    }

    public class BackTestConsumer : IConsumer<BacktestValue>
    {
        private readonly ILogger<BacktestResult> _logger;
        private readonly IBus bus;
        private readonly GrpcService service;

        private Dictionary<string, Type> types;

        public BackTestConsumer(ILogger<BacktestResult> logger, IBus bus, GrpcService service)
        {
            _logger = logger;
            {
                var type = typeof(BaseSignal);
                types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p))
                    .ToDictionary(x => x.Name);
            }

            this.bus = bus;
            this.service = service;
        }

        public List<Quote> GetQuetes(string symbol)
        {
            List<Quote> quotes = new List<Quote>();
            using (StreamReader sr = new StreamReader("./"+ symbol + ".csv"))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    var split = line.Split("|");
                    Quote q = new Quote
                    {
                        Date = DateTime.Parse(split[0]),
                        Open = decimal.Parse(split[1]),
                        High = decimal.Parse(split[2]),
                        Low = decimal.Parse(split[3]),
                        Close = decimal.Parse(split[4]),
                        Volume = decimal.Parse(split[6])
                    };
                    quotes.Add(q);
                }
            }
            return quotes;
        }

        public TestResult CalculateResult(BackTest bt)
        {
            TestResult result = new TestResult();

            result.WinTrades = bt.Trades.Where(x => x.Prifit > 0).Count();
            result.LossTrades = bt.Trades.Where(x => x.Prifit < 0).Count();

            var t = bt.Trades.Select(x => x.Prifit > 0).ToArray();

            var winCount = 0;
            var lossCount = 0;
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i])
                {
                    if (lossCount > 0) result.ConsequentLosses = Math.Max(result.ConsequentLosses, lossCount);
                    lossCount = 0;
                    winCount++;
                }
                else
                {
                    if (winCount > 0) result.ConsequentWins = Math.Max(result.ConsequentWins, winCount);
                    winCount = 0;
                    lossCount++;
                }
            }

            var wlr = (result.LossTrades == 0) ? 1 : (double)result.WinTrades / (result.LossTrades);
            var cwlr = (result.ConsequentLosses == 0) ? 1 : (double)result.WinTrades / (result.ConsequentLosses);
            result.Profit = (double)bt.Profit;

            result.Score = ((result.Profit * (wlr + cwlr) / (double)result.TotalTrades) * (double)result.WinTrades);

            return result;
        }

        public async Task Consume(ConsumeContext<BacktestValue> context)
        {
            var test = context.Message;
            List<Quote> quotes = await service.GetPrice(test.Symbol, test.Start, test.End);
            if(quotes.Count == 0) return;

            Stopwatch sw = Stopwatch.StartNew();
            BackTest bt = new BackTest(quotes);
            foreach (var item in test.StrategyArguments)
            {
                bt.AddSignal(types[item.Strategy]);
            }

            bt.SetArguments(test.StrategyArguments);
            bt.Run();
            if (bt.Profit == 0) return;

            var result = CalculateResult(bt);

            //_logger.LogInformation("{symbol}", test.Symbol);
            //_logger.LogInformation("Tests done in {elapsed}", sw.Elapsed);

            await bus.Publish(new BacktestResult(test.Symbol, result.Score, test.StrategyArguments, test.Start, test.End));
        }
    }
}

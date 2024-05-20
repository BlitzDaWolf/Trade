using Command.Context;
using Cronos;
using MassTransit;
using Shared;
using Shared.Attributes;
using Shared.Backtest;
using Shared.Backtest.Signals;
using System.Reflection;

namespace Command
{
    public class Worker : BackgroundService
    {
        private const string schedule = "*/5 * * * *"; // every hour
        private readonly CronExpression _cron;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<Worker> _logger;
        private readonly IBus _bus;
        private Dictionary<string, Type> types;

        public Worker(ILogger<Worker> logger, IBus bus, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _bus = bus;
            _cron = CronExpression.Parse(schedule);


            {
                var type = typeof(BaseSignal);
                types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => type.IsAssignableFrom(p))
                    .Where(x => x.Name != "BaseSignal")
                    .ToDictionary(x => x.Name);
            }
            _scopeFactory = scopeFactory;
        }

        public List<StrategyArgument> GetNextValues(Type strategyType)
        {
            var properties = strategyType.GetProperties();
            var filled = properties.Where(x => x.GetCustomAttribute(typeof(ParameterAttribute)) != null).ToArray();

            List<StrategyArgument> strategyArguments = new List<StrategyArgument>();
            foreach (var property in filled)
            {
                var attr = property.GetCustomAttribute<ParameterAttribute>();
                strategyArguments.Add(new StrategyArgument(strategyType.Name, property.Name, Random.Shared.Next((int)attr.MinValue, (int)attr.MaxValue)));
            }
            return strategyArguments;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started");
            while (!stoppingToken.IsCancellationRequested)
            {
                var n = DateTime.Now;
                n = new DateTime(n.Year, n.Month, n.Day);
                n=n.AddDays(-1);
                using (var scope = _scopeFactory.CreateScope())
                {
                    var utcNow = DateTime.UtcNow;
                    var nextUtc = _cron.GetNextOccurrence(utcNow);
                    // _logger.LogInformation("Next job batch: {time}", nextUtc!.Value);
                    // await Task.Delay(nextUtc.Value - utcNow, stoppingToken);

                    var _context = scope.ServiceProvider.GetRequiredService<CommandContext>();
                    var dates = _context.Results.GroupBy(x => x.End).Select(x => new { key = x.Key, count= x.Count() }).ToDictionary(x => x.key);

                    if (dates.Count > 0)
                    {
                        while (dates.ContainsKey(n) && dates[n].count >= 125 * types.Count)
                        {
                            n = n.AddDays(-1);
                        }
                    }

                    string[] symbols = _context.Prices.GroupBy(x => x.Symbol).Select(x => x.Key).ToArray();

                    for (int i = 0; i < 125; i++)
                    {
                        List<StrategyArgument> arguments = new List<StrategyArgument>();
                        foreach (var item in types)
                        {
                            if (Random.Shared.NextDouble() > 0.8)
                            {
                                arguments.AddRange(GetNextValues(item.Value));
                            }
                        }

                        if (arguments.Count > 1 && symbols.Length != 0)
                        {
                            await _bus.Publish(new BacktestValue(symbols[Random.Shared.Next(0, symbols.Length)], arguments.ToArray(), n.AddMonths(-2), n));
                        }
                    }
                }
            }
        }
    }
}

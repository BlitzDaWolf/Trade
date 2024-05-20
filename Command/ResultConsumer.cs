using Command.Context;
using Command.Models;
using MassTransit;
using Shared;
using System.Collections.Generic;
using static MassTransit.ValidationResultExtensions;

namespace Command
{
    public class ResultConsumer : IConsumer<BacktestResult>
    {
        private readonly ILogger<ResultConsumer> _logger;
        private readonly IBus _bus;
        private readonly CommandContext context;

        // private static List<BacktestResult> results = new List<BacktestResult>();

        public ResultConsumer(ILogger<ResultConsumer> logger, IBus bus, CommandContext context)
        {
            _logger = logger;
            _bus = bus;
            this.context = context;
        }

        private bool Same(StrategyArgumentModel[] left, StrategyArgumentModel[] right)
        {
            if( left.Length != right.Length) return false;
            foreach (var item in left)
            {
                if(!right.Select(x => x.FQN).Contains(item.FQN)) return false;
                var rValue = right.FirstOrDefault(x => x.FQN == item.FQN)!.Value;
                if (item.Value != rValue) return false;
            }
            return true;
        }

        public async Task Consume(ConsumeContext<BacktestResult> context)
        {
            // _logger.LogInformation("Recieved result");
            var result = context.Message;
            if (result.Profit == 0) return;

            var v  = this.context.Results.Where(x => x.Start == result.Start && x.End == result.End).Select(x => x.Id).ToList();
            var v2 = this.context.StrategyArguments.Where(x => v.Contains(x.ResultModelId)).GroupBy(x => x.ResultModelId).ToDictionary(x => x.Key);

            ResultModel resultModel = new ResultModel { End = result.End, Start = result.Start, Symbol = result.Symbol, PnL = result.Profit };
            var strategyAruments = result.StrategyArguments.Select(x => StrategyArgumentModel.FromGrpc(x, resultModel.Id)).ToList();

            if(!v2.Any(x => Same(x.Value.ToArray(), strategyAruments.ToArray())))
            {
                try
                {
                    // _logger.LogInformation("[{msgId}] {symbol}: {profit}", context.MessageId, result.Symbol, result.Profit);
                    this.context.Results.Add(resultModel);
                    this.context.StrategyArguments.AddRange(strategyAruments);
                    await this.context.SaveChangesAsync();
                }
                catch { }
            }
        }

        private async Task a(List<double> normalizedList, List<BacktestResult> ordered, int val)
        {
            var normalOrder = normalizedList.OrderDescending().ToList();

            var indx = normalizedList.IndexOf(normalOrder[val]);
            var left = true;
            if (normalizedList.Count == 2)
            {
                indx = 1;
                left = true;
            }
            else if (indx == normalizedList.Count-1)
            {
                left = true;
            }
            else if (indx == 0)
            {
                left = false;
            }
            else
            {
                try
                {
                    left = normalizedList[indx - 1] > normalizedList[indx + 1];
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Index: {idx}", indx);
                    throw new Exception("", ex);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(0.5));
            var curr = ordered[indx];
            StrategyArgument[] v = new StrategyArgument[curr.StrategyArguments.Length];
            if (left)
            {
                for (int i = 0; i < v.Length; i++)
                {
                    v[i] = new StrategyArgument(curr.StrategyArguments[i].Strategy,
                        curr.StrategyArguments[i].Argument,
                        (curr.StrategyArguments[i].value + ordered[indx - 1].StrategyArguments[i].value) / 2
                    );
                }
            }
            else
            {
                for (int i = 0; i < v.Length; i++)
                {
                    v[i] = new StrategyArgument(curr.StrategyArguments[i].Strategy,
                        curr.StrategyArguments[i].Argument,
                        (curr.StrategyArguments[i].value + ordered[indx + 1].StrategyArguments[i].value) / 2
                    );
                }
            }
            var n = DateTime.Now;
            await _bus.Publish(new BacktestValue("EURUSD", v, n.AddDays(-Random.Shared.Next(5, 60)), n));
        }
    }
}

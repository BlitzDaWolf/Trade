using Shared.Attributes;
using Skender.Stock.Indicators;
using System.Reflection;

namespace Shared.Backtest
{
    public class BackTest
    {
        List<Quote> quotes;
        Trade? activeTrade = null;
        List<Trade> trades = new List<Trade>();
        public List<Trade> Trades => trades;

        public decimal Profit => trades.Sum(x => x.Prifit);

        public BackTest(List<Quote> quotes)
        {
            this.quotes = quotes;
        }

        public List<BaseSignal> signals = new List<BaseSignal>();

        public void AddSignal<TSignal>() where TSignal : BaseSignal, new()
        {
            TSignal cs = new TSignal();
            signals.Add(cs);
        }

        public void AddSignal(Type signalType)
        {
            if(!signalType.IsSubclassOf(typeof(BaseSignal)))
            {
                throw new Exception("Not an valid signal");
            }
            BaseSignal signal = (BaseSignal)Activator.CreateInstance(signalType)!;
            signals.Add(signal);
        }

        public void Run()
        {
            signals.ForEach(x => x.Init(quotes));
            if (signals.Count == 0) throw new Exception("Not enough signals");
            while (signals.TrueForAll(x => !x.Done))
            {
                var offset = signals[0].GetOffset;
                if (activeTrade != null) activeTrade.Exit = quotes[signals[0].GetOffset].Close;
                signals.ForEach(x => x.Next());
                if (Profit < -10_000) break;
                var s = signals.Average(x => x.Signal);

                if (s > 0.5)
                {
                    Buy(1);
                }
                else if (s < -0.5)
                {
                    Sell(-1);
                }
                if (s == 0)
                {
                    // Close();
                }
            }
            Close();
        }

        public void SetArguments(StrategyArgument[] arguments)
        {
            foreach (var item in signals)
            {
                Type strategyType = item.GetType();
                var properties = strategyType.GetProperties();
                var filled = properties.Where(x => x.GetCustomAttribute(typeof(ParameterAttribute)) != null).ToArray();

                //Console.WriteLine(strategyType.Name);
                foreach (var property in filled)
                {
                    var attr = property.GetCustomAttribute<ParameterAttribute>();
                    var v = arguments.FirstOrDefault(x => x.Strategy == strategyType.Name && x.Argument == property.Name);

                    property.SetValue(item, v == null ? attr.Defaultvalue : (int)v.value);
                }
                //Console.WriteLine();
            }
        }

        private void Close()
        {
            if (activeTrade != null)
            {
                activeTrade.Active = false;
                trades.Add(activeTrade);
                activeTrade = null;
            }
        }

        private void Sell(double s)
        {
            var offset = signals[0].GetOffset;

            var amount = 5_000 / quotes[offset].Close;

            if (activeTrade != null)
            {
                if (activeTrade.TradeSide >= 0)
                {
                    activeTrade.Active = false;
                    trades.Add(activeTrade);
                    activeTrade = null;
                }
            }
            if (activeTrade == null)
            {
                activeTrade = new Trade { Active = true, Enter = quotes[offset].Close, Exit = quotes[offset].Close, TradeSide = (double)amount * s };
            }
        }

        private void Buy(double s)
        {
            var offset = signals[0].GetOffset;

            var amount = 5_000 / quotes[offset].Close;

            if (activeTrade != null)
            {
                if (activeTrade.TradeSide <= 0)
                {
                    activeTrade.Active = false;
                    trades.Add(activeTrade);
                    activeTrade = null;
                }
            }
            if (activeTrade == null)
            {
                activeTrade = new Trade { Active = true, Enter = quotes[offset].Close, Exit = quotes[offset].Close, TradeSide = (double)amount * s };
            }
        }

        public double GetLastSignal()
        {
            signals.ForEach(x => x.Init(quotes));
            signals.ForEach(x => x.SetLast());
            return signals.Average(x => x.Signal);
        }
    }

    public class Trade
    {
        public decimal Enter { get; set; }
        public decimal Exit { get; set; }

        public double TradeSide { get; set; }

        public decimal Prifit => (Exit - Enter) * (decimal)TradeSide;
        public bool Active { get; set; }
    }
}

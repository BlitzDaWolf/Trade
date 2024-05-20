using Shared.Backtest.Inducators;
using Skender.Stock.Indicators;

namespace Shared.Backtest
{
    public abstract class Indicator<T> where T : IReusableResult
    {
        public BaseSignal Signal { get; private set; }
        private IEnumerable<T> result = new List<T>();
        public IEnumerable<T> Values => result.Take(Signal.GetOffset + 1);
        public T? LastResult => result.Find(Signal.CurrentTime);

        internal void SetSignal(BaseSignal signal)
        {
            Signal = signal;
        }
        internal void calculate()
        {
            result = PreCalculate(Signal._bars);
        }
        public virtual IEnumerable<T> PreCalculate(List<Quote> bars) => new List<T>();
    }

    public class Indicator
    {
        readonly BaseSignal signal;

        public Indicator(BaseSignal signal)
        {
            this.signal = signal;
        }

        public ATR ATR(int LoopBackPeriods)
        {
            var a = new ATR { LoopBackPeriods = LoopBackPeriods };
            a.SetSignal(signal);
            a.calculate();
            return a;
        }
        public EMA EMA(int LoopBackPeriods)
        {
            var a = new EMA { loopBack = LoopBackPeriods };
            a.SetSignal(signal);
            a.calculate();
            return a;
        }
        public KDJ KDJ(int SingalPeriod, int SmoothPeriods)
        {
            var a = new KDJ { SingalPeriods = SingalPeriod, SmoothPeriods = SmoothPeriods };
            a.SetSignal(signal);
            a.calculate();
            return a;
        }
        public RSI RSI(int LoopBackPeriods)
        {
            var a = new RSI { loopBack = LoopBackPeriods };
            a.SetSignal(signal);
            a.calculate();
            return a;
        }
        public SMA SMA(int LoopBackPeriods)
        {
            var a = new SMA { loopBack = LoopBackPeriods };
            a.SetSignal(signal);
            a.calculate();
            return a;
        }
    }
}

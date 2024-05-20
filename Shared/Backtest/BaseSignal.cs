using Skender.Stock.Indicators;

namespace Shared.Backtest
{
    public abstract class BaseSignal
    {
        public IEnumerable<Quote> Bars => _bars.Take(current + 1);
        internal int GetOffset => current;

        public double Signal { get; set; } = 0;
        public bool Done => current >= _bars.Count - 1;

        public DateTime CurrentTime => _bars[current].Date;
        private int current = 0;

        internal List<Quote> _bars = new List<Quote>();

        public readonly Indicator Indicators;

        public BaseSignal()
        {
            Indicators = new Indicator(this);
        }

        public void Init(List<Quote> bars)
        {
            _bars = bars;
            OnStart();
        }

        public void Run()
        {
            for (current = 0; current < _bars.Count;)
            {
                Next();
            }
        }

        public void Next()
        {
            Signal = 0;
            OnBar();
            current++;
        }


        public virtual void OnStart() { }
        public virtual void OnBar() { }

        internal void SetLast()
        {
            Signal = 0;
            current = _bars.Count - 1;
            OnBar();
        }
    }
}

using Skender.Stock.Indicators;

namespace Shared.Backtest.Inducators
{
    public class SMA : Indicator<SmaResult>
    {
        public int loopBack { get; set; }

        public override IEnumerable<SmaResult> PreCalculate(List<Quote> bars)
        {
            return bars.GetSma(loopBack);
        }
    }
}

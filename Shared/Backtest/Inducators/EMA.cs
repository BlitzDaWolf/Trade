using Skender.Stock.Indicators;

namespace Shared.Backtest.Inducators
{
    public class EMA : Indicator<EmaResult>
    {
        public int loopBack { get; set; }

        public override IEnumerable<EmaResult> PreCalculate(List<Quote> bars)
        {
            return bars.GetEma(loopBack);
        }
    }
}

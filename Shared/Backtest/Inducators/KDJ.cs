using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Backtest.Inducators
{
    public class KDJ : Indicator<StochResult>
    {
        public int SmoothPeriods { get; set; }
        public int SingalPeriods { get; set; }
        public int LoopBack { get; set; }

        public override IEnumerable<StochResult> PreCalculate(List<Quote> bars)
        {
            return bars.GetStoch(LoopBack, SingalPeriods, SmoothPeriods);
        }
    }
}

using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Backtest.Inducators
{
    public class ATR : Indicator<AtrResult>
    {
        public int LoopBackPeriods { get; set; }

        public override IEnumerable<AtrResult> PreCalculate(List<Quote> bars)
        {
            return bars.GetAtr(LoopBackPeriods);
        }
    }
}

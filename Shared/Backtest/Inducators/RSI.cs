using Skender.Stock.Indicators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Backtest.Inducators
{
    public class RSI : Indicator<RsiResult>
    {
        public int loopBack { get; set; }

        public override IEnumerable<RsiResult> PreCalculate(List<Quote> bars)
        {
            return bars.GetRsi(loopBack);
        }
    }
}

using Shared.Attributes;
using Shared.Backtest.Inducators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Backtest.Signals
{
    public class RSISignal : BaseSignal
    {
        [Parameter(14, 5, 30)]
        public int RSISize { get; set; }
        private RSI rsi;


        public override void OnStart()
        {
            rsi = Indicators.RSI(RSISize);
        }

        public override void OnBar()
        {
            if (rsi.LastResult.Rsi > 70)
            {
                Signal = -1;
            }
            else if (rsi.LastResult.Rsi < 30)
            {
                Signal = 1;
            }
        }
    }
}

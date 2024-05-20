using Shared.Attributes;
using Shared.Backtest.Inducators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Backtest.Signals
{
    public class ATRCross : BaseSignal
    {
        ATR atr1;
        ATR atr2;

        [Parameter(14, 5, 500)]
        public int atr1Size { get; set; }
        [Parameter(28, 5, 500)]
        public int atr2Size { get; set; }

        public override void OnStart()
        {
            atr1 = Indicators.ATR(atr1Size);
            atr2 = Indicators.ATR(atr2Size);
        }

        public override void OnBar()
        {
            var a1 = atr1.Values.TakeLast(2).Select(x => x.Atr).ToArray();
            var a2 = atr2.Values.TakeLast(2).Select(x => x.Atr).ToArray();

            a1[0] = a1[0] - a2[0];

            if (a1[0] < 0 && a1[1] > 0)
            {
                Signal = -1;
            }
            else if (a1[0] < 1 && a1[1] > 1)
            {
                Signal = 1;
            }
        }
    }
}

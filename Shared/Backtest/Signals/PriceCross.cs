using Shared.Attributes;
using Shared.Backtest.Inducators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Backtest.Signals
{
    public class PriceCross : BaseSignal
    {
        [Parameter(20, 5, 50)]
        public int EMASize { get; set; }
        public EMA Ema { get; set; }


        public override void OnStart()
        {
            Ema = Indicators.EMA(EMASize);
        }

        public override void OnBar()
        {
            var s = Ema.Values.TakeLast(2).ToArray();
            var f = Bars.TakeLast(2).ToArray();

            if (s[0].Ema > (double)f[0].Close && s[1].Ema < (double)f[1].Close)
            {
                Signal = s[1].Ema < (double)f[1].Close ? 1 : -1;
            }
            if (s[0].Ema < (double)f[0].Close && s[1].Ema > (double)f[1].Close)
            {
                Signal = s[1].Ema < (double)f[1].Close ? 1 : -1;
            }
        }
    }
}

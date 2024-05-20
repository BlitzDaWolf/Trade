using Shared.Attributes;
using Shared.Backtest.Inducators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Backtest.Signals
{
    public class CrossSignal : BaseSignal
    {
        [Parameter(20, 5, 250)]
        public int SlowSize { get; set; }
        [Parameter(30, 5, 250)]
        public int FastSize { get; set; }

        private EMA slow;
        private EMA fast;

        public override void OnStart()
        {
            slow = Indicators.EMA(SlowSize);
            fast = Indicators.EMA(FastSize);
        }

        public override void OnBar()
        {
            var s = slow.Values.TakeLast(2).ToArray();
            var f = fast.Values.TakeLast(2).ToArray();

            if (s[0].Ema > f[0].Ema && s[1].Ema < f[1].Ema)
            {
                Signal = s[1].Ema < f[1].Ema ? 1 : -1;
            }
            if (s[0].Ema < f[0].Ema && s[1].Ema > f[1].Ema)
            {
                Signal = s[1].Ema < f[1].Ema ? 1 : -1;
            }
        }
    }
}

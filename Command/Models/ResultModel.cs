using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command.Models
{
    public class ResultModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Symbol { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double PnL { get; set; }
    }
}

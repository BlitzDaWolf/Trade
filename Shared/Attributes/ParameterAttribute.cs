using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Attributes
{
    
    public class ParameterAttribute : Attribute
    {
        public object? Defaultvalue { get; set; }
        public object? MinValue { get; set; }
        public object? MaxValue { get; set; }


        public ParameterAttribute(object? defaultvalue = null, object? minValue = null, object? maxValue = null)
        {
            Defaultvalue = defaultvalue;
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }
}

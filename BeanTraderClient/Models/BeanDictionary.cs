using BeanTrader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeanTrader.Models
{
    // This is only needed because WPF DataTemplates can't format generic types
    public class BeanDictionary : Dictionary<Beans, uint>
    {
        public BeanDictionary(Dictionary<Beans, uint> source) : base(source) { }
    }
}

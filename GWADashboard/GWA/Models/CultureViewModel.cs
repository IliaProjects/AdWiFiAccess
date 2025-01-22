using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GWA.Models
{
    public class CultureViewModel
    {
        public string text { get; set; }
        public string value { get; set; }

        public CultureViewModel(string _text, string _value)
        {
            text = _text;
            value = _value;
        }
    }
}

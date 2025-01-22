using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GWA.Models
{
    public class MenuItem
    {
        public MenuItem()
        {
            jsFunction = "";
            location = "";
            items = new List<MenuItem>();
        }

        [JsonIgnore]
        public int id { get; set; }

        public string text { get; set; }
        public bool disabled { get; set; }
        public string icon { get; set; }
        public List<MenuItem> items { get; set; }
        public string jsFunction { get; set; } // имя js функции, которая выполнится при клике на пунке меню
        public string location { get; set; } // url на который произойдет переход при клике на пункте меню
        public string permissionEnumText { get; set; }
    }
}

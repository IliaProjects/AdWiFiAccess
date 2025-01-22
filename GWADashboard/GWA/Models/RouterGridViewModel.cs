using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GWA.Models
{
    public class RouterGridViewModel
    {
        public string Id { get; set; }
        public string Nr { get; set; }
        public DateTime Online { get; set; }
        public string Model { get; set; }
        public string BusNr { get; set; }
        public string BusRoute { get; set; }
        public string PlacedTime { get; set; }
        public string Notes { get; set; }
    }
}

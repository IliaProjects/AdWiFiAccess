using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GWA.Data.Models
{
    public class Router
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string Nr { get; set; }

        [Required]
        public string Model { get; set; } = "Mikrotik";

        [Required]
        public DateTime PlacedTime{ get; set; } = new DateTime(1991, 1, 1, 12, 0, 0);

        [Required]
        public DateTime Online { get; set; } = new DateTime(1991, 1, 1, 12, 0, 0);

        public string Notes { get; set; }
        public virtual ICollection<BindingRouterBus> BindingRouterBuses { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
        public virtual ICollection<SessionArchieved> SessionsArchieve { get; set; }
    }
}

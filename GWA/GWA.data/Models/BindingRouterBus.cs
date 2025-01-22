using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GWA.Data.Models
{
    public class BindingRouterBus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string RouterId { get; set; }
        public virtual Router Router { get; set; }

        [Required]
        public string BusId { get; set; } 
        public virtual Bus Bus { get; set; }
    }
}

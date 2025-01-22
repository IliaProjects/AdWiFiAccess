using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GWA.Data.Models
{
    public class SessionArchieved
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string Mac { get; set; }

        [Required]
        public string Ip { get; set; }

        [Required]
        public DateTime ConnectedTime { get; set; }

        [Required]
        public DateTime StartingTime { get; set; }

        [Required]
        public DateTime TerminationTime { get; set; }

        [Required]
        public bool FullSession { get; set; }

        [Required]
        public EnumMadeAction.Action MadeAction { get; set; }

        public double InboundTrafficKiB { get; set; }

        public double OutboundTrafficKiB { get; set; }

        public string Notes { get; set; }



        [Required]
        public string OrderId { get; set; }

        [Required]
        public string OrderShareId { get; set; }

        [Required]
        public string RouterId { get; set; }



        public virtual Order Order { get; set; }
        public virtual OrderShare OrderShare { get; set; }
        public virtual Router Router { get; set; }
    }
}

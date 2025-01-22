using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace GWA.Data.Models
{
    public class Session
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

        public string Notes { get; set; }


        [Required]
        public EnumMadeAction.Action MadeAction { get; set; }



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

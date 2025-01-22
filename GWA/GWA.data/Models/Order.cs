using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GWA.Data.Models
{
    public enum OrderType
    {
        Video,
        Picture
    }

    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public OrderType Type { get; set; }

        [Required]
        public string Amount { get; set; }

        [Required]
        public int Counter { get; set; } = 0;

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime PlacedTime { get; set; }

        [Required]
        public bool IsExecuted { get; set; } = false;

        public string RedirectUrl { get; set; }

        public DateTime ExecutedTime { get; set; } = new DateTime(1991, 1, 1, 12, 0, 0);

        public string Notes { get; set; }
        public virtual ICollection<Session> Sessions{ get; set; }
        public virtual ICollection<SessionArchieved> SessionsArchieve { get; set; }
    }
}

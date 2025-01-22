using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GWA.Data.Models
{
    public class OrderShare
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string Amount { get; set; }

        [Required]
        public int Counter { get; set; }

        [Required]
        public string Picture { get; set; }

        public string Text { get; set; }

        [Required]
        public DateTime PlacedTime { get; set; }

        [Required]
        public bool IsExecuted { get; set; }

        public string Url { get; set; }

        public DateTime ExecutedTime { get; set; }

        public string Notes { get; set; }
        public virtual ICollection<Session> Sessions{ get; set; }
        public virtual ICollection<SessionArchieved> SessionsArchieve { get; set; }
    }
}

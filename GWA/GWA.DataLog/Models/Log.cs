using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GWA.DataLog.Models
{
    public  enum EventTypes
    {
        Information=1,
        Warning=2,
        Error=3,
        GWA=4,
        GWAHttpLogging = 5
    }

    public class Log
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [StringLength(50)]
        public string Action { get; set; }

        [StringLength(50)]
        public string Controller { get; set; }

        [Required]
        public EventTypes EventType { get; set; }

        [Required]
        public string Message { get; set; }                
    }
}

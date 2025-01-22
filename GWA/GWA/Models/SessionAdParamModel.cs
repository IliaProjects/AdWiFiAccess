using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GWA.Data.Models;

namespace GWA.Models
{
    public class SessionAdParamModel
    {
        [Required]
        public string SessionId { get; set; }

        [Required]
        public string Content { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GWA.Data.Models;

namespace GWA.Models
{
    public class SessionParamModel
    {
        [Required]
        public string SessionHoverId { get; set; }

        [Required]
        public string SharePicture { get; set; }

        [Required]
        public string ShareLink { get; set; }

    }
}

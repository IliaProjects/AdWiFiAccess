using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GWA.Data.Models
{
    public class UserConnection
    {
        [Key]
        [StringLength(50)]
        public string SessionId { get; set; }

        [Required]
        public bool IsRemember { get; set; }

        [Required]
        public bool IsOnline { get; set; }

        [Required]
        public string UserId { get; set; }

        [StringLength(1024)]
        public string UserAgent { get; set; }

        [StringLength(39)]
        public string UserIp { get; set; }

        public DateTime LastRequestDate { get; set; }

        [StringLength(50)]
        public string LastRequestAction { get; set; }

        [StringLength(50)]
        public string LastRequestController { get; set; }

        [StringLength(1024)]
        public string LastRequestRawUrl { get; set; }

        public string LastRequestPostParams { get; set; }

        public virtual ApplicationUser User { get; set; }

    }
}

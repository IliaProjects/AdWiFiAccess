using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GWA.Data.Models
{
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string RoleId { get; set; }

        [Required]
        public string OperationId { get; set; }

        [Required]
        public bool Permitted { get; set; }


        public virtual ApplicationRole Role { get; set; }
        public virtual Operation Operation { get; set; }
    }
}

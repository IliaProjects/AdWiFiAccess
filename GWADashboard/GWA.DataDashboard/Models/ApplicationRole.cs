using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GWA.Data.Models
{
    public class ApplicationRole : IdentityRole
    {
        [Required]
        public string UserId { get; set; } // кто создал роль

        [Required]
        public string Alias { get; set; }   // здесь будем хранить название роли, а в поле NAME и NORMALIZEDNAME будем хранить название в виде префикс_alias

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}

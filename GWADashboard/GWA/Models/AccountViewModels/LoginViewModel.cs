using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GWA.Models.AccountViewModels
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Requred field")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Requred field")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}

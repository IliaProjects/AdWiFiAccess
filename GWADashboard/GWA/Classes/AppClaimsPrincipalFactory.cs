using GWA.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GWA.Classes
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private IHttpContextAccessor _contextAccessor;

        public AppClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> optionsAccessor, IHttpContextAccessor contextAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {

            var principal = await base.CreateAsync(user);

            ((ClaimsIdentity)principal.Identity).AddClaims(new[]
            {
                new Claim("UserId", user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim("RememberMe", user.RememberMe.ToString()),
                new Claim("SessionId", _contextAccessor.HttpContext.Session.Id)
            });

            return principal;
        }
    }
}

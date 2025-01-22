using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GWA.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace GWA.Controllers
{
    [Authorize]
    public class SessionsController : GWAController
    {

        private readonly AppDbContext _db;
        public SessionsController(AppDbContext db, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _db = db;
        }

        [HttpGet]
        [Route("~/sessions", Name = "SessionsIndex")]
        public IActionResult SessionsIndex()
        {
            return PartialView();
        }
    }
}
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
    public class PresessionsController : GWAController
    {

        private readonly AppDbContext _db;
        public PresessionsController(AppDbContext db, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _db = db;
        }


        [HttpGet]
        [Route("~/presessions", Name = "PresessionsIndex")]
        public IActionResult PresessionsIndex()
        {
            return PartialView();
        }
    }
}
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
    public class BusesController : GWAController
    {

        private readonly AppDbContext _db;
        public BusesController(AppDbContext db, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _db = db;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
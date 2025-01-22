using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GWA.Controllers.api
{
    public class TestController : Controller
    {
        [HttpGet]
        [Route("/test", Name = "Test")]
        public IActionResult Index()
        {
            return View("TestIndex");
        }
    }
}
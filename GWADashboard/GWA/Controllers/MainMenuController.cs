using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GWA.Controllers
{
    [Authorize]
    public class MainMenuController : GWAController
    {
        public MainMenuController(IServiceProvider serviceProvider) : base(serviceProvider) { }
        public IActionResult Index()
        {
            var view = View();
            return view;
        }
    }
}
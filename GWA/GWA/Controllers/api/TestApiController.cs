using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using GWA.Models;
using GWA.Data;
using GWA.Data.Models;
using GWA.Classes;
using GWA.DataLog;
using GWA.DataLog.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;

namespace GWA.Controllers.Api
{
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("/api/test", Name = "TestApi")]
    public class TestApiController : GWAController
    {

        public enum PutParam
        {
            AdIsWatched,
            SessionStarted,
            SessionDropped
        }

        private readonly AppDbContext _db;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TestApiController(IWebHostEnvironment webHostEnvironment, AppDbContext db, ILogger<SessionApiController> logger, 
                                    IConfiguration configuration, IServiceProvider s) : base(s)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _db = db;
            _configuration = configuration;
        }

        [HttpGet]
        public object Get(string token, string userid)
        {
            Console.WriteLine("|------------------------------");
            Console.WriteLine("|  Facebook user id: " + userid);
            Console.WriteLine("|------------------------------");
            Console.WriteLine("|  User token id: " + token);
            Console.WriteLine("|------------------------------");
            return 1;
        }
    }
}

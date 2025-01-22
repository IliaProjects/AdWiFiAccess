using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using GWA.Classes;
using GWA.Data;
using GWA.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GWA.Controllers.api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/presessions", Name = "PresessionsApi")]
    public class PresessionsApiController : GWAController
    {
        private AppDbContext _db;
        private ILogger _logger;
        public PresessionsApiController(AppDbContext db, IServiceProvider serviceProvider, ILogger<BusesApiController> logger) : base(serviceProvider)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public object Get(string route, DataSourceLoadOptions loadOptions)
        {
            List<SessionHover> sessions = new List<SessionHover>();

            sessions = _db.SessionsHover.ToList();

            return DataSourceLoader.Load(sessions, loadOptions);
        }
    }
}
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
    [Route("api/buses", Name = "BusesApi")]
    public class BusesApiController : GWAController
    {
        private class Route
        {
            public Route(string val, int amount)
            {
                route = val;
                busamount = amount;
            }
            public string route { get; set; }

            public int busamount { get; set; }
        }
        private AppDbContext _db;
        private ILogger _logger;
        public BusesApiController(AppDbContext db, IServiceProvider serviceProvider, ILogger<BusesApiController> logger) : base(serviceProvider)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        [Route("/api/buses/routes", Name = "BusesRoutesApi")]
        public object GetRoutes(DataSourceLoadOptions loadOptions)
        {
            var d = new Dictionary<string, int>();
            foreach (var x in _db.Buses.Select(s => s.Route).ToList())
            {
                if (d.ContainsKey(x))
                    d[x]++;
                else
                    d[x] = 1;
            }

            List<Route> routelist = new List<Route>();
            foreach (var value in d)
            {
                routelist.Add(new Route(value.Key, value.Value));
            }

            return DataSourceLoader.Load(routelist, loadOptions);
        }

        [HttpGet]
        public object Get(string route, DataSourceLoadOptions loadOptions)
        {
            List<Bus> buses;
            if (route != null)
            {
                buses = _db.Buses.AsQueryable().Where(w => w.Route == route).ToList();
            }
            else
            {
                buses = _db.Buses.ToList();
            }
            return DataSourceLoader.Load(buses, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var bus = new Bus();
            JsonConvert.PopulateObject(values, bus);

            if (!TryValidateModel(bus))
                return BadRequest(ModelState.GetFullErrorMessage());

            _db.Buses.Add(bus);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex, false));
            }

            return Ok();
        }

        [HttpPut]
        async public Task<IActionResult> Put(string key, string values)
        {
            var bus = _db.Buses.First(a => a.Id == key);
            JsonConvert.PopulateObject(values, bus);

            if (!TryValidateModel(bus))
                return BadRequest(ModelState.GetFullErrorMessage());

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex, false));
            }

            return Ok();
        }

        [HttpDelete]
        async public Task<IActionResult> Delete(string key)
        {
            var bus = _db.Buses.First(a => a.Id == key);

            _db.Buses.Remove(bus);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return BadRequest(Utils.GetFullError(ex, false));
            }

            return Ok();
        }
    }
}
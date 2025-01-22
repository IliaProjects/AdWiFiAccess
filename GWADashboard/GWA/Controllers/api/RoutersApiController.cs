using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using GWA.Classes;
using GWA.Models;
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
    [Route("api/routers", Name = "RoutersApi")]
    public class RoutersApiController : GWAController
    {
        private AppDbContext _db;
        private ILogger _logger;
        public RoutersApiController(AppDbContext db, IServiceProvider serviceProvider, ILogger<RoutersApiController> logger) : base(serviceProvider)
        {
            _logger = logger;
            _db = db;
        }
        [HttpGet]
        public object Get(string busRoute, DataSourceLoadOptions loadOptions)
        {
            List<RouterGridViewModel> routers = new List<RouterGridViewModel>();

            if(busRoute != null)
            {
                foreach (var bus in _db.Buses.Where(w => w.Route == busRoute).ToList())
                {
                    var bind = _db.BindingRouterBuses.FirstOrDefault(f => f.BusId == bus.Id);
                    if (bind != null)
                    {
                        var router = _db.Routers.FirstOrDefault(f => f.Id == bind.RouterId);
                        routers.Add(new RouterGridViewModel
                        {
                            Id = router.Id,
                            Nr = router.Nr,
                            BusNr = bind.Bus.Nr,
                            BusRoute = bind.Bus.Route,
                            Online = router.Online,
                            PlacedTime = router.PlacedTime.ToString("dd/MM/yyyy HH:mm"),
                            Model = router.Model,
                            Notes = router.Notes,
                        });
                    }
                }
            }
            else
            {
                var rr = _db.Routers.ToList();
                foreach (var r in rr) {
                    var bus = _db.Buses.Find(_db.BindingRouterBuses.FirstOrDefault(f => f.RouterId == r.Id).BusId);
                    routers.Add(new RouterGridViewModel {
                        Id = r.Id,
                        Nr = r.Nr,
                        BusNr = bus.Nr,
                        BusRoute = bus.Route,
                        Online = r.Online,
                        PlacedTime = r.PlacedTime.ToString("dd/MM/yyyy HH:mm"),
                        Model = r.Model,
                        Notes = r.Notes,
                    });
                }
            }

            return DataSourceLoader.Load(routers, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var router = new Router();

            JsonConvert.PopulateObject(values, router);

            if (!TryValidateModel(router))
                return BadRequest(ModelState.GetFullErrorMessage());

            _db.Routers.Add(router);

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
            var router = _db.Routers.First(a => a.Id == key);
            var routerModel = new RouterGridViewModel();
            JsonConvert.PopulateObject(values, routerModel);

            router.Notes = routerModel.Notes;

            var bus = _db.Buses.FirstOrDefault(f => f.Nr == routerModel.BusNr);
            if (bus != null)
            {
                _db.BindingRouterBuses.FirstOrDefault(f => f.RouterId == router.Id).BusId = bus.Id;
            }
            else
            {
                return BadRequest("Троллейбуса с таким номером не существует");
            }

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

            var router = _db.Routers.Single(s => s.Id == key);

            _db.Routers.Remove(router);

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
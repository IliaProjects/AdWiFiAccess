using System;
using System.Collections.Generic;
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
    [Route("api/advideos", Name ="AdVideosApi")]
    public class AdVideosApiController : GWAController
    {
        private AppDbContext _db;
        private ILogger _logger;
        public AdVideosApiController(AppDbContext db, IServiceProvider serviceProvider, ILogger<AdVideosApiController> logger) : base(serviceProvider)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            var videos = _db.Orders.Where(w => w.Type == Data.Models.OrderType.Video).ToList();
            return DataSourceLoader.Load(videos, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var advideo = new Order();
            JsonConvert.PopulateObject(values, advideo);

            advideo.Type = OrderType.Video;
            advideo.PlacedTime = Utils.TimezoneTime(+3);

            if (!TryValidateModel(advideo))
                return BadRequest(ModelState.GetFullErrorMessage());

            _db.Orders.Add(advideo);

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
            var advideo = _db.Orders.First(a => a.Id == key && a.Type == OrderType.Video);
            JsonConvert.PopulateObject(values, advideo);

            if (!TryValidateModel(advideo))
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
            var advideo = _db.Orders.First(a => a.Id == key && a.Type == OrderType.Video);

            _db.Orders.Remove(advideo);

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
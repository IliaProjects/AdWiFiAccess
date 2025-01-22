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
    [Route("api/adpictures", Name ="AdPicturesApi")]
    public class AdPicturesApiController : GWAController
    {
        private AppDbContext _db;
        private ILogger _logger;
        public AdPicturesApiController(AppDbContext db, IServiceProvider serviceProvider, ILogger<AdPicturesApiController> logger) : base(serviceProvider)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            var pictures = _db.Orders.Where(w => w.Type == Data.Models.OrderType.Picture).ToList();
            return DataSourceLoader.Load(pictures, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var adpicture = new Order();
            JsonConvert.PopulateObject(values, adpicture);

            adpicture.Type = OrderType.Picture;
            adpicture.PlacedTime = Utils.TimezoneTime(+3);

            if (!TryValidateModel(adpicture))
                return BadRequest(ModelState.GetFullErrorMessage());

            _db.Orders.Add(adpicture);

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
            var adpicture = _db.Orders.First(a => a.Id == key && a.Type == OrderType.Picture);
            JsonConvert.PopulateObject(values, adpicture);

            if (!TryValidateModel(adpicture))
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
            var adpicture = _db.Orders.First(a => a.Id == key && a.Type == OrderType.Picture);

            _db.Orders.Remove(adpicture);

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
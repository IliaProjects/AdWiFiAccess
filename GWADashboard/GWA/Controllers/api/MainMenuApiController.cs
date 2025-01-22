using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using GWA.Classes;
using GWA.Data;
using GWA.Data.Models;
using GWA.DataDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace GWA.Controllers.api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/mainmenu", Name = "MainMenuApi")]
    public class MainMenuApiController : GWAController
    {
        private DashboardDbContext _db;
        private ILogger _logger;
        public MainMenuApiController(DashboardDbContext db, IServiceProvider serviceProvider, ILogger<MainMenuApiController> logger) : base(serviceProvider)
        {
            _logger = logger;
            _db = db;
        }
        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            var menu = _db.MainMenu.AsQueryable().Select(s => new
            {
                Id = s.Id,
                ParentId = s.ParentId,
                Text = s.Text,
                TextRo = s.TextRo,
                TextEn = s.TextEn,
                Icon = s.Icon,
                JsFunction = s.JsFunction,
                Location = s.Location,
                OrderId = s.OrderId,
                IsActive = s.IsActive,
                PermissionEnumText = s.PermissionEnumText,
                HasItems = _db.MainMenu.Count(m => m.ParentId == s.Id) > 0
            }).OrderBy(s => s.ParentId).ThenBy(s => s.OrderId);

            var list = menu.ToList();
            return DataSourceLoader.Load(menu, loadOptions);
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {
            var menuItem = new MainMenu();

            JsonConvert.PopulateObject(values, menuItem);

            if (!TryValidateModel(menuItem))
                return BadRequest(ModelState.GetFullErrorMessage());

            // вычисляем OrderId
            menuItem.OrderId = _db.MainMenu.Where(s => s.ParentId == menuItem.ParentId).Count() + 1;

            _db.MainMenu.Add(menuItem);

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
        async public Task<IActionResult> Put(int key, string values)
        {
            var menuItem = _db.MainMenu.First(a => a.Id == key);
            JsonConvert.PopulateObject(values, menuItem);

            if (!TryValidateModel(menuItem))
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
        async public Task<IActionResult> Delete(int key)
        {
            var children = _db.MainMenu.Where(s => s.ParentId == key).ToList();

            foreach (var child in children)
            {
                await Delete(child.Id);
            }

            var menuItem = _db.MainMenu.Where(s => s.Id == key);

            _db.MainMenu.RemoveRange(menuItem);
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

        [HttpPost]
        [Route("/api/mainmenu/reorder")]
        async public Task<IActionResult> Reorder(int draggingRowKey, int targetRowKey, bool shiftPressed)
        {
            var target = _db.MainMenu.Find(targetRowKey);
            var source = _db.MainMenu.Find(draggingRowKey);

            if (shiftPressed)
            {
                source.ParentId = target.Id;
                source.OrderId = _db.MainMenu.Count(s => s.ParentId == target.Id);
            }
            else
            {
                int targetOrderId = target.OrderId;
                int sourceOrderId = source.OrderId;
                int targetParentId = target.ParentId;
                int sourceParentId = source.ParentId;

                target.OrderId = sourceOrderId;
                source.OrderId = targetOrderId;
                target.ParentId = sourceParentId;
                source.ParentId = targetParentId;
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

        [HttpPost]
        [Route("/api/mainmenu/changeIcon")]
        async public Task<IActionResult> changeIcon(string cellIdForIcon, string result)
        {
            var id = Int32.Parse(cellIdForIcon);

            var allData = (from c in _db.MainMenu
                           where c.Id == id
                           select c).First();

            allData.Icon = result.ToString();

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
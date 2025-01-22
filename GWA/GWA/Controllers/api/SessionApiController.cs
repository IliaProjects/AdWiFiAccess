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
using System.Web;

namespace GWA.Controllers.Api
{
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("/api/session", Name = "SessionApi")]
    public class SessionApiController : GWAController
    {

        public enum PutParam
        {
            Shared,
            SessionStarted
        }

        private readonly AppDbContext _db;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SessionApiController(IWebHostEnvironment webHostEnvironment, AppDbContext db, ILogger<SessionApiController> logger, 
                                    IConfiguration configuration, IServiceProvider s) : base(s)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _db = db;
            _configuration = configuration;
        }

        [HttpGet]
        public object Get(string mac)
        {
            return 1;
        }

        [HttpPost]
        async public Task<IActionResult> Post(string values)
        {

            JObject obj = JObject.Parse(values);

            var model = new
            {
                LinkLoginOnly = obj.Value<string>("LinkLoginOnly"),
                LinkOrigEsc = obj.Value<string>("LinkOrigEsc"),
                RouterId = Utils.GetRouterId(_db, obj.Value<string>("RouterNr")),
                MacEsc = obj.Value<string>("MacEsc"),
                Mac = obj.Value<string>("Mac"),
                Ip = obj.Value<string>("Ip"),
            };

            #region валидация данных
            if (!_db.Routers.Any(a => a.Id == model.RouterId))
            {
                _logger.LogCritical("Error: wi-fi point is not recognized. Router: " + model.RouterId);
                return BadRequest("Ошибка: точка wi-fi не распознана. Проблема будет исправлена в ближайшее время. Приносим извинения");
            }

            if (model.Mac.Count(s => s == ':') != 5 || model.Ip.Count(s => s == '.') != 3)
            {
                _logger.LogError("Error: invalid MAC: " + model.Mac + " or IP: " + model.Ip);
                return BadRequest("Ваше устройство или Ваш Ip адрес не распознан. Попробуйте переподключиться");
            }
            #endregion

            //Если существует сессия, и пользователь зашел повторно
            if (_db.SessionsHover.Any(s => s.Mac == model.Mac && s.RouterId == model.RouterId))
            {
                return Ok(_db.SessionsHover.First(s => s.Mac == model.Mac && s.RouterId == model.RouterId).Id);
            }

            #region Выборка ролика/картинки (var orderId)
            string orderId;
            var orders = _db.Orders.ToList();

            //Есть ли невыполненные заказы
            if (orders.Any(a => a.IsExecuted == false))
            {
                orders = orders.Where(w => w.IsExecuted == false).ToList();
                orderId = orders[new Random().Next(orders.Count)].Id;
            }
            else 
            {
                //Есть ли в принципе заказы
                if (orders.Any())
                {
                    orderId = orders[new Random().Next(orders.Count)].Id;
                }
                else
                {
                    return BadRequest("No orders available. Try again later");
                }
            }
            #endregion

            #region Выборка репоста (var orderShareId)
            string orderShareId;
            var ordersShare = _db.OrdersShare.ToList();

            //Есть ли невыполненные заказы
            if (ordersShare.Any(a => a.IsExecuted == false))
            {
                ordersShare = ordersShare.Where(w => w.IsExecuted == false).ToList();
                orderShareId = ordersShare[new Random().Next(ordersShare.Count)].Id;
            }
            else
            {
                //Есть ли в принципе заказы
                if (ordersShare.Any())
                {
                    orderShareId = ordersShare[new Random().Next(ordersShare.Count)].Id;
                }
                else
                {
                    return BadRequest("No orders available. Try again later");
                }
            }
            #endregion

            string trialLinkPartI = model.LinkLoginOnly + "?dst=";
            string trialLinkPartII = "&username=T-" + model.MacEsc;

            var sessionHover = new SessionHover
            {
                Mac = model.Mac,
                Ip = model.Ip,
                RouterId = model.RouterId,
                OrderId = orderId,
                OrderShareId = orderShareId,

                TrialLinkPartI = trialLinkPartI,
                TrialLinkPartII = trialLinkPartII,
                ConnectedTime = Utils.MoldovaTime(),

                MadeAction = EnumMadeAction.Action.Nothing
            };

            _db.SessionsHover.Add(sessionHover);
            await _db.SaveChangesAsync();
                    
            //Console log
            Utils.ConsoleConnectedLog(sessionHover.Mac, sessionHover.Ip, Utils.MoldovaTime(), model.RouterId);

            return Ok(sessionHover.Id);
        }

        //Пользователь успешно сделал репост на facebook
        [HttpPut]
        async public Task<IActionResult> IActionResult(string sessionId)
        {
            if (sessionId == "default" || sessionId == null)
            {

                //Console log
                Console.WriteLine("default entry");
                return Ok("https://google.com/");
            }

            var sessionHover = _db.SessionsHover.Single(s => s.Id == sessionId);

            var session = new Session() {
                Mac = sessionHover.Mac,
                Ip = sessionHover.Ip,
                ConnectedTime = sessionHover.ConnectedTime,
                StartingTime = Utils.MoldovaTime(),
                MadeAction = EnumMadeAction.Action.Shared,
                OrderId = sessionHover.OrderId,
                OrderShareId = sessionHover.OrderShareId,
                RouterId = sessionHover.RouterId,
            };



            var trialLink = sessionHover.TrialLinkPartI + HttpUtility.UrlEncode(_db.OrdersShare.Single(s => s.Id == session.OrderShareId).Url) + sessionHover.TrialLinkPartII;

            Console.WriteLine(trialLink);

            _db.Sessions.Add(session);
            _db.SessionsHover.Remove(sessionHover);

            await _db.SaveChangesAsync();

            return Ok(trialLink);
        }

        [HttpDelete]
        public IActionResult Delete(string param)
        {
            return Ok();
        }
    }
}

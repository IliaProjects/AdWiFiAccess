using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GWA.Models;
using GWA.Data;
using GWA.Classes;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace GWA.Controllers
{
    public class AdBoardController : GWAController
    {

        private readonly AppDbContext _db;
        public AdBoardController(AppDbContext db, IServiceProvider s) : base (s)
        {
            _db = db;
        }

        [HttpGet]
        [Route("~/adboard", Name = "AdBoardIndex")]
        public IActionResult AdBoardIndex(string sessionId)
        {
            string _sessionId = "default";
            var order = new Data.Models.Order();
            var orders = _db.Orders.ToList();

            //Если это реальная сессия с микротика
            if (sessionId != null && sessionId != "default")
            {
                //Меняем sessionId и пробуем найти сессию и соответствующий ей ролик
                _sessionId = sessionId;
                try
                {
                    var orderId = _db.SessionsHover.Single(s => s.Id == _sessionId).OrderId;
                    order = orders.Single(s => s.Id == orderId);
                }
                catch (Exception ex)
                {
                    return BadRequest("Invalid session id");
                }
            }
            //Если это просто заход на сайт, например, с домашнего компьютера
            else
            {
                //Выбираем любой ролик
                order = orders[new Random().Next(orders.Count)];
            }
            
            var ad = new SessionAdParamModel
            {
                SessionId = _sessionId,
                Content = order.Content,
            };

            switch (order.Type)
            {
                case Data.Models.OrderType.Video:
                    {
                        return View("AdBoardVideoIndex", ad);
                    }
                case Data.Models.OrderType.Picture:
                    {
                        return View("AdBoardPictureIndex", ad);
                    }
                default:
                    {
                        return BadRequest("invalid order type");
                    }
            }
        }

        [HttpGet]
        [Route("~/adboard/img", Name = "ImgAdBoardIndex")]
        public IActionResult ImgAdBoardIndex()
        {
            string _sessionId = "default";
            var order = new Data.Models.Order();
            var orders = _db.Orders.Where(w => w.Type == Data.Models.OrderType.Picture).ToList();

            order = orders[new Random().Next(orders.Count)];

            var ad = new SessionAdParamModel
            {
                SessionId = _sessionId,
                Content = order.Content,
            };

            return View("AdBoardPictureIndex", ad);
        }
    }
}

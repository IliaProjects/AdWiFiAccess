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

namespace GWA.Controllers
{
    public class SessionController : GWAController
    {

        private readonly AppDbContext _db;
        public SessionController(AppDbContext db, IServiceProvider s) : base (s)
        {
            _db = db;
        }


        public IActionResult Index(string id)
        {
            string sessionId = "default";

            var order = new Data.Models.Order();
            var orders = _db.Orders.ToList();

            var orderShare = new Data.Models.OrderShare();
            var ordersShare = _db.OrdersShare.ToList();

            if (id != null)
            {
                //Меняем sessionId и пробуем найти сессию и соответствующий ей ролик
                sessionId = id;
                try
                {
                    var sessionHover = _db.SessionsHover.Single(s => s.Id == id);

                    var orderId = sessionHover.OrderId;
                    order = orders.Single(s => s.Id == orderId);

                    var orderShareId = sessionHover.OrderShareId;
                    orderShare = ordersShare.Single(s => s.Id == orderShareId);
                }
                catch (Exception ex)
                {
                    return BadRequest("Invalid session id");
                }
            }
            else
            {
                //Выбираем любой ролик и любой репост
                order = orders[new Random().Next(orders.Count)];
                orderShare = ordersShare[new Random().Next(ordersShare.Count)];
            }

            var ad = new SessionParamModel
            {
                SessionHoverId = sessionId,
                ShareLink = orderShare.Url,
                SharePicture = orderShare.Picture,
            };

            return View("Index", ad);
        }
    }
}

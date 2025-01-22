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
    [Route("api/mikrotik", Name = "MikrotikApi")]
    public class MikrotikApiController : GWAController
    {

        private readonly AppDbContext _db;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MikrotikApiController(IWebHostEnvironment webHostEnvironment, AppDbContext db, ILogger<MikrotikApiController> logger,
                                    IConfiguration configuration, IServiceProvider s) : base(s)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _db = db;
            _configuration = configuration;
        }

        //Сессия начата
        [HttpGet]
        public object Get(string mac, string router)
        {

            Console.WriteLine(router + " " + mac);
            //Есть ли сессия с mac и стартовала ли она в течение последней минуты
            if (_db.Sessions.Any(a => a.Mac == mac && a.RouterId == Utils.GetRouterId(_db, router) && a.StartingTime > Utils.MoldovaTime().AddMinutes(-1)))
            {
                //Console log
                Console.WriteLine("MikrotikApi GET: Connection submitted");
                return 1;
            }
            else
            {
                //Console log
                _logger.LogCritical("User " + mac + " illegal connection. Router: " + router);
                return 2;
            }
        }

        [HttpPost]
        [Route("/api/mikrotik/regbusrouter")]
        async public Task<int> Reg(string param)
        {
            JObject obj = JObject.Parse(param);
            var model = new
            {
                RouterNr = obj.Value<string>("router"),
                BusNr = obj.Value<string>("bus"),
                BusRoute = obj.Value<string>("busroute"),
                Model = obj.Value<string>("model"),
                Placed = obj.Value<DateTime>("placed"),
                Notes = obj.Value<string>("notes"),
                Password = obj.Value<string>("password")
            };



            Console.WriteLine("Router :" + model.RouterNr);
            Console.WriteLine("Bus :" + model.BusNr);
            Console.WriteLine("BusRoute :" + model.BusRoute);
            Console.WriteLine("Model :" + model.Model);
            Console.WriteLine("Placed :" + model.Placed);
            Console.WriteLine("Notes :" + model.Notes);
            Console.WriteLine("Password :" + model.Password);

            if (obj.Value<string>("password") != _configuration.GetSection("AppConfigurations").GetValue<string>("MikrotikPassword"))
            {
                _logger.LogCritical("Mikrotik Reg Authorize Failed");
                return 2;
            }

            #region Валидация данных

            if (model.RouterNr.Length != 4 || model.BusNr.Length != 4 ||  model.BusRoute.Length != 2 || model.Placed == null)
            {
                return 2;
            }

            foreach (char c in model.RouterNr)
            {
                if (!char.IsDigit(c))
                {
                    return 2;
                }
            }

            foreach (char c in model.BusNr)
            {
                if (!char.IsDigit(c))
                {
                    return 2;
                }
            }

            foreach (char c in model.BusRoute)
            {
                if (!char.IsDigit(c))
                {
                    return 2;
                }
            }
            #endregion

            //Если нет троллейбуса - регистрируем
            var bus = _db.Buses.FirstOrDefault(f => f.Nr == model.BusNr);
            if (bus == null)
            {
                bus = new Bus
                {
                    Nr = model.BusNr,
                    Route = model.BusRoute,
                };
                _db.Buses.Add(bus);

                try
                {
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(Utils.GetFullError(ex));
                    return 2;
                }
            }

            var router = new Router()
            {
                Nr = model.RouterNr,
                Model = model.Model,
                PlacedTime = model.Placed,
                Notes = model.Notes,
                Online = Utils.MoldovaTime(),
            };
            _db.Routers.Add(router);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(Utils.GetFullError(ex));
                return 2;
            }

            _db.BindingRouterBuses.Add(new BindingRouterBus
            {
                BusId = bus.Id,
                RouterId = router.Id,
            });
            await _db.SaveChangesAsync();

            _logger.LogInformation("Router " + router.Nr + " has been registrated in the system.");
            return 1;
        }

        //Стартап (есть ли роутер, и если да, остались ли в БД сессии онлайн от него)
        [HttpPost]
        [Route("/api/mikrotik/startup")]
        async public Task<int> Startup(string nr, string password)
        {
            Console.WriteLine("Startup request nr:" + nr + "   password: " + password);
            if (password != _configuration.GetSection("AppConfigurations").GetValue<string>("MikrotikPassword")) 
            {
                return 1;
            }
            if (_db.Routers.Any(a => a.Nr == nr))
            {
                Console.WriteLine("Found router nr: " + nr);

                var routerId = Utils.GetRouterId(_db, nr);
                if (_db.Sessions.Any(a => a.RouterId == routerId))
                {
                    var sessions = _db.Sessions.Where(w => w.RouterId == routerId);

                    Console.WriteLine("Found sessions amount: " + sessions.Count());
                    foreach (var session in sessions)
                    {
                        var sessionToArchieve = new SessionArchieved
                        {
                            Mac = session.Mac,
                            Ip = session.Ip,
                            ConnectedTime = session.ConnectedTime,
                            StartingTime = session.StartingTime,
                            TerminationTime = Utils.MoldovaTime(),
                            FullSession = false,
                            MadeAction = session.MadeAction,
                            Notes = "Record archieved on router startup",
                            RouterId = session.RouterId,
                            OrderId = session.OrderId,
                            OrderShareId = session.OrderShareId,
                        };
                        _db.SessionsArchieved.Add(sessionToArchieve);
                        _db.Sessions.Remove(session);
                    }
                    await _db.SaveChangesAsync();
                }
                return 1;
            }
            else
            {
                return 2;
            }
        }

        //Планировщик
        [HttpPost]
        async public Task<int> Post(string param)
        {
            JObject obj = JObject.Parse(param);
            var nr = obj.Value<string>("nr");

            if (obj.Value<string>("password") != _configuration.GetSection("AppConfigurations").GetValue<string>("MikrotikPassword"))
            {
                return 1;
            }

            try
            {
                var router = _db.Routers.Single(s => s.Nr == nr);
                router.Online = Utils.MoldovaTime();
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to find router nr " + nr);
            }

            await SessionsCleanerSingleton.getInstance().Ping(_db, nr);

            return 0;
        }



        //Сессия окончена
        [HttpPost]
        [Route("/api/mikrotik/endsession")]
        async public Task<int> IActionResult(string param)
        {
            //Console log
            Console.WriteLine(param);
            JObject obj = JObject.Parse(param);
            var mac = obj.Value<string>("mac");
            var router = obj.Value<string>("router");
            double intrafik = Math.Round(Convert.ToDouble(obj.Value<string>("intrafik")) / 1024, 3);
            double outtrafik = Math.Round(Convert.ToDouble(obj.Value<string>("outtrafik")) / 1024, 3);
            DateTime dateTime = Convert.ToDateTime(obj.Value<string>("date") + " " + obj.Value<string>("time"));
            Console.WriteLine("DateTime logout: " + dateTime);
            //Console.WriteLine(intrafik + " | " + outtrafik + " | " + mac);
            try
            {
                var session = _db.Sessions.Single(s => s.Mac == mac && s.StartingTime.Year > 2010);
                var sessionToArchieve = new SessionArchieved()
                {
                    Mac = session.Mac,
                    Ip = session.Ip,
                    ConnectedTime = session.ConnectedTime,
                    StartingTime = session.StartingTime,
                    FullSession = true,
                    TerminationTime = dateTime,
                    InboundTrafficKiB = intrafik,
                    OutboundTrafficKiB = outtrafik,
                    OrderId = session.OrderId,
                    RouterId = session.RouterId,
                    MadeAction = session.MadeAction,
                    OrderShareId = session.OrderShareId,
                };

                _db.SessionsArchieved.Add(sessionToArchieve);
                _db.Sessions.Remove(session);

                await _db.SaveChangesAsync();

                //Console log
                Utils.ConsoleDisonnectedLog(mac, session.Ip, Utils.MoldovaTime(), session.RouterId);
            }
            catch (Exception ex)
            {
                //Console log
                Console.WriteLine("MikrotikApi PUT: failed");

                //Если сессия есть, то она не одна, и нужно обработать все
                if (_db.Sessions.Any(a => a.Mac == mac && a.StartingTime.Year > 2010))
                {
                    var sessions = _db.Sessions.Where(a => a.Mac == mac && a.StartingTime.Year > 2010).ToList();
                    var last = sessions.Single(s => s.ConnectedTime == Utils.GetLatestDate(sessions.Select(s => s.ConnectedTime).ToList()));

                    //Сохраняем последнюю сессию
                    var lastSessionToArchieve = new SessionArchieved()
                    {
                        Mac = last.Mac,
                        Ip = last.Ip,
                        ConnectedTime = last.ConnectedTime,
                        StartingTime = last.StartingTime,
                        FullSession = true,
                        TerminationTime = dateTime,
                        InboundTrafficKiB = intrafik,
                        OutboundTrafficKiB = outtrafik,
                        OrderId = last.OrderId,
                        RouterId = last.RouterId,
                        MadeAction = last.MadeAction,
                        OrderShareId = last.OrderShareId,
                    };
                    _db.SessionsArchieved.Add(lastSessionToArchieve);
                    await _db.SaveChangesAsync();

                    //Сохраняем остальные сессии со ссылкой на последнюю
                    foreach (var session in sessions)
                    {
                        if (session != last)
                        {
                            var sessionToArchieve = new SessionArchieved()
                            {
                                Mac = session.Mac,
                                Ip = session.Ip,
                                ConnectedTime = session.ConnectedTime,
                                StartingTime = session.StartingTime,
                                FullSession = false,
                                TerminationTime = dateTime,
                                OrderId = session.OrderId,
                                RouterId = session.RouterId,
                                MadeAction = session.MadeAction,
                                OrderShareId = session.OrderShareId,
                                Notes = "ref -> " + lastSessionToArchieve.Id,
                            };
                            _db.SessionsArchieved.Add(lastSessionToArchieve);
                            await _db.SaveChangesAsync();
                        }
                    }

                    //Удаляем сессии из Sessions
                    foreach (var session in sessions)
                    {
                        _db.Sessions.Remove(session);
                        await _db.SaveChangesAsync();
                    }
                }
            }
            return 1;
        }

        [HttpDelete]
        public IActionResult Delete(string values)
        {
            Console.WriteLine("Delete request");
            Console.WriteLine(values);
            return Ok();
        }

        [HttpPost]
        [Route("/api/mikrotik/logvalue", Name = "LogValue")]
        async public Task<int> LogValue(string value)
        {
            Console.WriteLine("LogValue: " + value);
            return 0;
        }

        [HttpPost]
        [Route("/api/mikrotik/logdate", Name = "LogDate")]
        async public Task<int> LogDate(string values)
        {
            JObject obj = JObject.Parse(values);
            var model = new
            {
                date = obj.Value<string>("date"),
                time = obj.Value<string>("time"),
            };
            Console.WriteLine("String DateTime: " + model.date + " " + model.time);
            Console.WriteLine("DateTime DateTime: " + Convert.ToDateTime(model.date + " " + model.time));
            Console.WriteLine("DateTime Local: " + Utils.MoldovaTime());
            return 0;
        }
    }
}

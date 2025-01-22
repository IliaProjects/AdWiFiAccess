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
    [Route("/api/adboard", Name = "AdBoardApi")]
    public class AdBoardApiController : GWAController
    {

        public enum PutParam
        {
            AdIsWatched,
            SessionStarted,
            SessionDropped
        }

        private readonly AppDbContext _db;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdBoardApiController(IWebHostEnvironment webHostEnvironment, AppDbContext db, ILogger<SessionApiController> logger, 
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


        [HttpPut]
        async public Task<IActionResult> IActionResult(string sessionId, PutParam action)
        {
            if (sessionId == "default")
            {

                //Console log
                Console.WriteLine("default entry");
                return Ok("https://google.com/");
            }

            switch (action)
            {
                case PutParam.AdIsWatched:
                    {
                        try
                        {
                            var sessionHover = _db.SessionsHover.Single(s => s.Id == sessionId);
                            switch (sessionHover.MadeAction)
                            {
                                case EnumMadeAction.Action.Nothing:
                                    {
                                        var order = _db.Orders.Single(s => s.Id == sessionHover.OrderId);
                                        order.Counter++;
                                        sessionHover.MadeAction = EnumMadeAction.Action.AdWatched;
                                        await _db.SaveChangesAsync();
                                        break;
                                    }
                                case EnumMadeAction.Action.AdWatched:
                                    {
                                        _logger.LogInformation("User " + sessionId + " refreshed video.");
                                        break;
                                    }
                                default:
                                    {
                                        var order = _db.Orders.Single(s => s.Id == sessionHover.OrderId);
                                        order.Counter++;
                                        sessionHover.MadeAction = EnumMadeAction.Action.AdWatched;
                                        await _db.SaveChangesAsync();
                                        break;
                                    }
                            }

                            string redirectUrl = _db.Orders.Single(s => s.Id == sessionHover.OrderId).RedirectUrl;

                            if (redirectUrl == null || redirectUrl.Length < 5)
                            {
                                redirectUrl = "https://google.com/";
                            }

                            string trialLink = sessionHover.TrialLinkPartI + HttpUtility.UrlEncode(redirectUrl) + sessionHover.TrialLinkPartII;

                            return Ok(trialLink);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest("Invalid request");
                        }
                    };
                case PutParam.SessionStarted:
                    {
                        try
                        {
                            var sessionHover = _db.SessionsHover.Single(s => s.Id == sessionId && s.MadeAction == EnumMadeAction.Action.AdWatched);

                            var session = new Session()
                            {
                                Mac = sessionHover.Mac,
                                Ip = sessionHover.Ip,
                                ConnectedTime = sessionHover.ConnectedTime,
                                StartingTime = Utils.MoldovaTime(),
                                MadeAction = sessionHover.MadeAction,
                                OrderId = sessionHover.OrderId,
                                OrderShareId = sessionHover.OrderShareId,
                                RouterId = sessionHover.RouterId,
                            };

                            _db.Sessions.Add(session);
                            _db.SessionsHover.Remove(sessionHover);

                            await _db.SaveChangesAsync();

                            return Ok();
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    };
                default:
                    {
                        return BadRequest("Invalid request");
                    }
            }
        }

        [HttpDelete]
        public IActionResult Delete(string param)
        {
            return Ok();
        }
    }
}
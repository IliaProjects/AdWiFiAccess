using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using GWA.Models;
using GWA.Data;
using GWA.DataLog;
using GWA.Classes;

namespace GWA.Controllers
{
    public class GWAController : Controller
    {
        IServiceProvider _serviceProvider;

        public GWAController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string currentAction = "";
            string currentController = "";
            string requestPath = "";
            string requestPostParam = "";
            string method = "";
            string requesrUserAgent = "";
            string userIp = "";

            //IConfiguration configuration = _httpContextAccessor.HttpContext.RequestServices.GetService<IConfiguration>();

            IConfiguration configuration = _serviceProvider.GetService<IConfiguration>();
            LogDbContext logDb = _serviceProvider.GetService<LogDbContext>();
            ILogger logger = (_serviceProvider.GetService<ILoggerFactory>()).CreateLogger("GWAController_OnActionExecutionAsync");

            // если стоит в настройках признак, что нужно логировать и пользователь аутентифицирован
            if (configuration.GetSection("AppConfigurations").GetValue<int>("UseHttpLogging") == 1) //&& User.Identity.IsAuthenticated)
            {

                currentAction = ControllerContext.RouteData.Values["action"].ToString();
                currentController = ControllerContext.RouteData.Values["controller"].ToString();

                requestPath = Request.Path;
                requestPostParam = ((Request.ContentType == null)) ? null : Newtonsoft.Json.JsonConvert.SerializeObject(Request.Form);

                method = Request.Method;
                requesrUserAgent = Request.Headers["User-Agent"].ToString();
                userIp = ControllerContext.HttpContext.Connection.RemoteIpAddress.ToString();

                // записываем информация о запросе в логи
                DataLog.Models.Log log = new DataLog.Models.Log
                {
                    Action = currentAction,
                    Controller = currentController,
                    Date = Utils.MoldovaTime(),
                    EventType = DataLog.Models.EventTypes.GWAHttpLogging,
                    Message = "",
                };
                logDb.Logs.Add(log);
                logDb.SaveChangesAsync();
            }

            return base.OnActionExecutionAsync(context, next);
        }
    }
}

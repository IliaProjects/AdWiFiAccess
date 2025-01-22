using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Threading.Tasks;
using GWA.Models;
using Microsoft.AspNetCore.Http;
using GWA.Data;
using GWA.DataDashboard;
using Microsoft.AspNetCore.Localization;
using System.Security.Claims;
using GWA.Data.Models;
using Microsoft.Extensions.Logging;

namespace GWA.Classes
{
    public class Utils
    {
        public static string GetFullError(Exception ex, bool showCallStack = true)
        {
            StringBuilder result = new StringBuilder();
            if (ex != null)
            {
                do
                {
                    result.AppendLine(ex.Message);

                    if (showCallStack)
                        result.Append(ex.StackTrace);

                    ex = ex.InnerException;

                }
                while (ex != null);
            }
            return result.ToString().Trim();
        }
        public static DateTime TimezoneTime(double timezoneOffset)
        {
            var time = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
            time = time.AddHours(timezoneOffset);
            return time;
        }

        public static void ConsoleDisonnectedLog(string mac, string ip, DateTime dateTime, string routerId)
        {
            Console.WriteLine("----------User disconnected----------");
            Console.WriteLine("|  Date:    " + dateTime.Hour + ":" + dateTime.Minute + ":" + dateTime.Second);
            Console.WriteLine("|  Mac:     " + mac);
            Console.WriteLine("|  Router:  " + routerId);
            Console.WriteLine("|  Ip:      " + ip);
        }

        public static void ConsoleConnectedLog(string mac, string ip, DateTime dateTime, string routerId)
        {
            Console.WriteLine("-----------New connectiton-----------");
            Console.WriteLine("|  Date:    " + dateTime.Hour + ":" + dateTime.Minute + ":" + dateTime.Second);
            Console.WriteLine("|  Mac:     " + mac);
            Console.WriteLine("|  Router:  " + routerId);
            Console.WriteLine("|  Ip:      " + ip);
        }

        public static DateTime GetLatestDate(List<DateTime> dates)
        {
            List<DateTime> _dates = dates;
            for (int i = 0; i < _dates.Count - 1; i++)
            {   
                for (int j = i + 1; j > 0; j--)
                {
                    //меняем ">" на "<" для обратной сортировки
                    if (_dates[j] > _dates[j - 1])
                    {
                        var temp = _dates[j];
                        _dates[j] = _dates[j - 1];
                        _dates[j - 1] = temp;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return _dates.First();
        }
        public static List<MenuItem> GetUserMenu(HttpContext context, DashboardDbContext db)
        {
            string lang = "ru";
            var langCookie = context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];

            if (langCookie != null)
            {
                string[] arr = langCookie.Split('|');
                lang = (arr[0].Split('='))[1];
            }

            List<MenuItem> menu = new List<MenuItem>();


            var rootItems = db.MainMenu.Where(s => s.ParentId == 0 && s.IsActive).OrderBy(s => s.OrderId).ToList();

            foreach (var item in rootItems)
            {
                switch (lang)
                {
                    case "ro":
                        menu.Add(new MenuItem
                        {
                            id = item.Id,
                            jsFunction = item.JsFunction,
                            location = item.Location,
                            icon = item.Icon,
                            text = item.TextRo,
                            permissionEnumText = item.PermissionEnumText
                        });
                        break;
                    case "en":
                        menu.Add(new MenuItem
                        {
                            id = item.Id,
                            jsFunction = item.JsFunction,
                            location = item.Location,
                            icon = item.Icon,
                            text = item.TextEn,
                            permissionEnumText = item.PermissionEnumText
                        });
                        break;
                    default:
                        menu.Add(new MenuItem
                        {
                            id = item.Id,
                            jsFunction = item.JsFunction,
                            location = item.Location,
                            icon = item.Icon,
                            text = item.Text,
                            permissionEnumText = item.PermissionEnumText
                        });
                        break;
                }
            }

            var items = db.MainMenu.Where(s => s.ParentId != 0 && s.IsActive).OrderBy(s => s.OrderId).ToList();

            bool isFinish;

            do
            {
                isFinish = true;
                foreach (var item in items.ToList())
                {
                    // смотрим если item уже присутствует в нашем menu то берем следующий
                    if (IsItemExists(menu, item.Id))
                        continue;

                    var parentItem = GetParentItem(menu, item.ParentId);
                    if (parentItem == null)
                    {
                        isFinish = false;
                    }
                    else
                    {
                        switch (lang)
                        {
                            case "ro":
                                parentItem.items.Add(new MenuItem
                                {
                                    id = item.Id,
                                    text = item.TextRo,
                                    icon = item.Icon,
                                    jsFunction = item.JsFunction,
                                    location = item.Location,
                                    permissionEnumText = item.PermissionEnumText
                                });
                                break;
                            case "en":
                                parentItem.items.Add(new MenuItem
                                {
                                    id = item.Id,
                                    text = item.TextEn,
                                    icon = item.Icon,
                                    jsFunction = item.JsFunction,
                                    location = item.Location,
                                    permissionEnumText = item.PermissionEnumText
                                });
                                break;
                            default:
                                parentItem.items.Add(new MenuItem
                                {
                                    id = item.Id,
                                    text = item.Text,
                                    icon = item.Icon,
                                    jsFunction = item.JsFunction,
                                    location = item.Location,
                                    permissionEnumText = item.PermissionEnumText
                                });
                                break;
                        }

                    }
                }
            } while (!isFinish);



            // удаляем пункт меню РАЗРАБОТЧИК если пользователь не Developer
            if (!context.User.IsInRole("developer"))
            {
                var devMenuItem = menu.Where(s => s.id == 1).FirstOrDefault();
                menu.Remove(devMenuItem);
            }

            return menu;
        }
        #region Private Methods
        private static bool IsItemExists(List<MenuItem> menu, int id)
        {
            bool result = false;

            foreach (var item in menu)
            {
                if (item.id == id)
                {
                    result = true;
                    break;
                }

                if (item.items.Count > 0)
                {
                    result = IsItemExists(item.items, id);
                    if (result)
                        break;
                }
            }

            return result;
        }

        private static MenuItem GetParentItem(List<MenuItem> menu, int parentId)
        {
            MenuItem result = null;

            foreach (var item in menu)
            {
                if (item.id == parentId)
                {
                    result = item;
                    break;
                }

                if (item.items.Count > 0)
                {
                    result = GetParentItem(item.items, parentId);
                    if (result != null)
                        break;
                }
            }

            return result;
        }
        #endregion

        public static bool CheckPermission(DashboardDbContext db, ClaimsPrincipal user, string permissionEnumText, ILogger logger)
        {
            if (user.IsInRole("developer"))
                return true;

            if (permissionEnumText == null)
                return false;

            try
            {
                var operationId = db.Operations.FirstOrDefault(s => s.EnumName.Equals(permissionEnumText)).Id;
                var userRoles = db.UserRoles.AsQueryable().Where(s => s.UserId == user.Identity.GetUserId()).Select(s => new { s.RoleId }).ToList();

                return db.Permissions.Any(s => userRoles.Any(r => r.RoleId == s.RoleId) && s.OperationId == operationId && s.Permitted == true);
            }
            catch (Exception ex)
            {
                logger.LogError($"CheckPermission for user: {user.Identity.GetUserName()}, permissionEnum: {permissionEnumText}." + Utils.GetFullError(ex));
                return false;
            }
        }

        public static bool CheckPermission(DashboardDbContext db, ClaimsPrincipal user, Permission permission)
        {
            if (user.IsInRole("developer"))
                return true;

            var permissionEnumText = Enum.GetName(typeof(Permission), permission);

            var operationId = db.Operations.FirstOrDefault(s => s.EnumName.Equals(permissionEnumText)).Id;
            var userRoles = db.UserRoles.AsQueryable().Where(s => s.UserId == user.Identity.GetUserId()).Select(s => new { s.RoleId }).ToList();

            return db.Permissions.Any(s => userRoles.Any(r => r.RoleId == s.RoleId) && s.OperationId == operationId);
        }
        public static string GetRouterId(AppDbContext _db, string routerNr)
        {
            return _db.Routers.Single(s => s.Nr == routerNr).Id;
        }

        public static string GetRouterNr(AppDbContext _db, string routerId)
        {
            return _db.Routers.Single(s => s.Id == routerId).Nr;
        }
    }
}

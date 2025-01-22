using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
        public static DateTime MoldovaTime()
        {
            int offset = 2;
            var time = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
            time = time.AddHours(offset);
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



        public static string GetRouterId(Data.AppDbContext _db, string routerNr)
        {
            return _db.Routers.Single(s => s.Nr == routerNr).Id;
        }

        public static string GetRouterNr(Data.AppDbContext _db, string routerId)
        {
            return _db.Routers.Single(s => s.Id == routerId).Nr;
        }

        public static void testDI(IConfiguration _configuration)
        {
            Console.WriteLine(_configuration.GetSection("AppConfigurations").GetValue<string>("MikrotikPassword"));
        }
    }
}

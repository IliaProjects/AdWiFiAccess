using GWA.Data;
using GWA.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GWA.Classes
{
    public class SessionsCleanerSingleton
    {
        private static SessionsCleanerSingleton instance;

        Dictionary<string, DateTime> _dictionary = new Dictionary<string, DateTime>();

        protected SessionsCleanerSingleton()
        {
        }

        public static SessionsCleanerSingleton getInstance()
        {
            if (instance == null)
                instance = new SessionsCleanerSingleton();
            return instance;
        }

        public async Task<bool> Ping(AppDbContext _db, string routerNr)
        {
            if (_dictionary.ContainsKey(routerNr))
            {
                TimeSpan timeSpan = Utils.MoldovaTime().Subtract(_dictionary[routerNr]);
                // Если с последней проверки прошло больше получаса
                if (timeSpan.TotalSeconds > 1800)
                {
                    foreach(var session in _db.SessionsHover.Where(w => w.RouterId == Utils.GetRouterId(_db, routerNr)))
                    {
                        //Если есть пред-сессии, подсоединенные больше, чем полчаса назад
                        if (Utils.MoldovaTime().Subtract(session.ConnectedTime).TotalSeconds > 1800)
                        {
                            var sessionArchieve = new SessionHoverArchieved
                            {
                                Mac = session.Mac,
                                Ip = session.Ip,
                                ConnectedTime = session.ConnectedTime,
                                OrderId = session.OrderId,
                                OrderShareId = session.OrderShareId,
                                RouterId = session.RouterId,
                                MadeAction = session.MadeAction
                            };

                            _db.SessionsHoverArchieved.Add(sessionArchieve);
                            _db.SessionsHover.Remove(session);
                        }
                    }
                    await _db.SaveChangesAsync();
                    _dictionary[routerNr] = Utils.MoldovaTime();
                }
            }
            else
            {
                _dictionary.Add(routerNr, Utils.MoldovaTime());
            }

            return true;
        }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseTrackerWeb.Services
{
    public static class SessionExtensions
    {
        static ISession _session;

        public static void SetBool(this ISession session, string key, bool value)
        {
            _session = session;

            if (value)
            {
                session.SetString(key, true.ToString());
            }
            else
            {
                session.SetString(key, false.ToString());
            }
        }

        public static bool GetBool(string key)
        {
            if(_session is null)
            {
                return false;
            }
            if(_session.GetString(key) == true.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void ClearSession()
        {
            _session = null;
        }
    }
}

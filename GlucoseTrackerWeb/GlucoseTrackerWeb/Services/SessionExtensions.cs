using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseTrackerWeb.Services
{
    public static class SessionExtensions
    {
        public static void SetBool(this ISession session, string key, bool value)
        {
            if (value)
            {
                session.SetString(key, true.ToString());
            }
            else
            {
                session.SetString(key, false.ToString());
            }
        }

        public static bool GetBool(this ISession session, string key)
        {
            if(session is null)
            {
                return false;
            }
            if(session.GetString(key) == true.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

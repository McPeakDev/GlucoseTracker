using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseAPI.Models.Entities
{
    public class Credentials
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public override string ToString()
        {
            JObject jCreds = JObject.FromObject(this);
            string jsonCreds = jCreds.ToString();
            return jsonCreds;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using GlucoseTrackerWeb.Models;

namespace GlucoseTrackerWeb.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private string _hash = "";

        public APIController()
        {
            using (StreamReader reader = System.IO.File.OpenText(@"appsettings.json"))
            {
                JToken j = JObject.Parse(reader.ReadToEnd())["Miscellanous"];
                _hash = j["Hash"].ToString();
            }
        }

        // GET: api/API
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/API/5
        [HttpGet("{token}/{email}/{password}", Name = "Get")]
        public string Get(string token, string email, string password)
        {
            if (_hash == token)
            {
                return "Success";
            }
            else
            {
                return "Failure";
            }
        }

        // POST: api/API
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/API/5
        [HttpPut("{pass}")]
        public void Put(string pass, [FromBody] Credentials creds)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

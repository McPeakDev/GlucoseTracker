using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GlucoseAPI.Models.Entities;
using Newtonsoft.Json.Linq;

namespace GlucoseTrackerApp.Services
{
    public class RestService
    {
        private HttpClient _client;
        private readonly string  _baseAddress;
        private HttpResponseMessage _response;
        private string _data;

        public RestService()
        {
            _client = new HttpClient();
            _baseAddress = "http://glucosetracker.duckdns.org:8080/api/values/";

        }

        public async Task<Patient> LoginAsync(Credentials creds)
        {
            StringContent loginContent = new StringContent(JObject.FromObject(creds).ToString(), Encoding.UTF8, "application/json");
            _response =  await _client.PostAsync(new Uri(_baseAddress), loginContent);
            _data = await _response.Content.ReadAsStringAsync();
            Patient user = JsonConvert.DeserializeObject<Patient>(_data.ToString());
            return user;
        }

        public async void RegisterAsync(Patient patient)
        {
            string test = JObject.FromObject(patient).ToString();

            StringContent registerContent = new StringContent(JObject.FromObject(patient).ToString(), Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(new Uri(_baseAddress + "Create/"), registerContent);
            _data = await _response.Content.ReadAsStringAsync();
        }
    }
}
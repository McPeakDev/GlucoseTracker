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
            _baseAddress = "http://glucosetracker.duckdns.org:8080/api/";

        }

        public RestService(string token)
        {
            _client = new HttpClient();
            _baseAddress = "http://glucosetracker.duckdns.org:8080/api/";

        }

        public async Task<Patient> LoginAsync(Credentials creds)
        {
            string credsString = JObject.FromObject(creds).ToString();

            StringContent loginContent = new StringContent(JObject.FromObject(creds).ToString(), Encoding.UTF8, "application/json");
            _response =  await _client.PostAsync(new Uri(_baseAddress) + "Token/", loginContent);
            _data = await _response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Add("token", _data);
            _response = await _client.PostAsync(new Uri(_baseAddress) + "Read/", null);
            _data = await _response.Content.ReadAsStringAsync();

            Patient user = JsonConvert.DeserializeObject<Patient>(_data.ToString());
            return user;
        }

        public async void RegisterAsync(PatientCreationBundle patientCreationBundle)
        {
            string test = JObject.FromObject(patientCreationBundle).ToString();

            StringContent registerContent = new StringContent(JObject.FromObject(patientCreationBundle).ToString(), Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(new Uri(_baseAddress + "Create/"), registerContent);
            _data = await _response.Content.ReadAsStringAsync();
        }
    }
}
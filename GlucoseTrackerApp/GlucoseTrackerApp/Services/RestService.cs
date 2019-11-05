using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GlucoseAPI.Models.Entities;
using Newtonsoft.Json.Linq;
using Android.Widget;

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
            _client.DefaultRequestHeaders.Add("token", token);
            _baseAddress = "http://glucosetracker.duckdns.org:8080/api/";

        }

        #region Authentication
        public async Task<string> LoginAsync(Credentials creds)
        {
            //Retrive the patients's token
            StringContent loginContent = new StringContent(JObject.FromObject(creds).ToString(), Encoding.UTF8, "application/json");
            _response =  await _client.PostAsync(new Uri(_baseAddress) + "Auth/", loginContent);
            _data = await _response.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Add("token", _data);

            //Return the Token
            return _data;
        }
        #endregion

        public async void RegisterAsync(PatientCreationBundle patientCreationBundle)
        {
            //Serialize the patientCreationBundle and send it to the API.
            StringContent registerContent = new StringContent(JObject.FromObject(patientCreationBundle).ToString(), Encoding.UTF8, "application/json");
            await _client.PostAsync(new Uri(_baseAddress + "Patient/Create/"), registerContent);
        }

        public async Task<Patient> ReadPatient()
        {
            _response = await _client.PostAsync(new Uri(_baseAddress + "Patient/Read/"), null);
            _data = await _response.Content.ReadAsStringAsync();

            Patient patient = JsonConvert.DeserializeObject<Patient>(_data);
            return patient;
        }

        public async void UpdatePatientAsync(Patient patient)
        {
            //Serialize the patientCreationBundle and send it to the API.
            StringContent patientContent = new StringContent(JObject.FromObject(patient).ToString(), Encoding.UTF8, "application/json");
            await _client.PutAsync(new Uri(_baseAddress + "Patient/Update/"), patientContent);
        }

        public async void CreatePatientData(PatientData patientData)
        {
            //Serialize the patientCreationBundle and send it to the API.
            StringContent createDataContent = new StringContent(JObject.FromObject(patientData).ToString(), Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(new Uri(_baseAddress + "Data/Create/"),createDataContent);
            _data = await _response.Content.ReadAsStringAsync();
        }
    }
}
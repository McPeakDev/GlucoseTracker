﻿using System;
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

        #region Constructors
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
        #endregion

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

        #region Registration
        public async void RegisterAsync(PatientCreationBundle patientCreationBundle)
        {
            //Serialize the patientCreationBundle and send it to the API.
            StringContent registerContent = new StringContent(JObject.FromObject(patientCreationBundle).ToString(), Encoding.UTF8, "application/json");
            await _client.PostAsync(new Uri(_baseAddress + "Patient/Create/"), registerContent);
        }
        #endregion

        #region Read, Update, and Delete Patient
        public async Task<Patient> ReadPatientAsync()
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

        public async void DeletePatientAsync()
        {
            //Serialize the patientCreationBundle and send it to the API.
            await _client.DeleteAsync(new Uri(_baseAddress + "Patient/Delete/"));
        }
        #endregion

        #region Create, Read, Update, and Delete Patient Data
        public async void CreatePatientData(PatientData patientData)
        {
            //Serialize the patientCreationBundle and send it to the API.
            StringContent createDataContent = new StringContent(JObject.FromObject(patientData).ToString(), Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(new Uri(_baseAddress + "Data/Create/"), createDataContent);
            _data = await _response.Content.ReadAsStringAsync();
        }

        public async Task<PatientData> ReadPatientDataAsync()
        {
            _response = await _client.PostAsync(new Uri(_baseAddress + "Data/Read/"), null);
            _data = await _response.Content.ReadAsStringAsync();

            PatientData patientData = JsonConvert.DeserializeObject<PatientData>(_data);
            return patientData;
        }

        public async void UpdatePatientAsync(PatientData patientData)
        {
            //Serialize the patientCreationBundle and send it to the API.
            StringContent patientContent = new StringContent(JObject.FromObject(patientData).ToString(), Encoding.UTF8, "application/json");
            await _client.PutAsync(new Uri(_baseAddress + "Data/Update/"), patientContent);
        }

        public async void DeletePatientAsync(PatientData patientData)
        {
            //Serialize the patientCreationBundle and send it to the API.
            StringContent patientContent = new StringContent(JObject.FromObject(patientData).ToString(), Encoding.UTF8, "application/json");
            await _client.PostAsync(new Uri(_baseAddress + "Data/Update/"), patientContent);
        }
        #endregion

        #region Query USDA
        public async Task<int> FindMealDataAsync(string query)
        {
            StringContent queryContent = new StringContent("{ \"generalSearchInput\": \"" + $"{query}" + "\" }", Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(new Uri("https://api.nal.usda.gov/fdc/v1/search?api_key=1l3ujGaRMh4QaOuxKEyqYSwNIDx0dSr9tmKycClk"), queryContent);
            _data = await _response.Content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(_data);

            int fdcId = (int) jObject.SelectToken("foods[0].fdcId");
            return fdcId;
        }

        public async Task<float> ReadMealDataAsync(int fdcId)
        {
            _response = await _client.GetAsync(new Uri($"https://api.nal.usda.gov/fdc/v1/{fdcId}?api_key=1l3ujGaRMh4QaOuxKEyqYSwNIDx0dSr9tmKycClk"));
            _data = await _response.Content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(_data);

            JArray nutrients = jObject.SelectToken("foodNutrients") as JArray;

            foreach (var nutrient in nutrients)
            {
                string nutrentName = (string)(nutrient.SelectToken("nutrient.name"));
                if (nutrentName.Contains("Carbohydrate"))
                {
                    return ((float)nutrient.SelectToken("amount"));
                }
            }

            return 0.00F;
        }
        #endregion

        #region Add MealItem
        public async void CreateMealItem(MealItem mealItem)
        {
            //Serialize the patientCreationBundle and send it to the API.
            StringContent mealItemContent = new StringContent(JObject.FromObject(mealItem).ToString(), Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(new Uri(_baseAddress + "Meal/Create/"), mealItemContent);
            _data = await _response.Content.ReadAsStringAsync();
        }
        #endregion
    }
}
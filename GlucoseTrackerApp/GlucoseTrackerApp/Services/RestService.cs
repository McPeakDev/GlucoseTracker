﻿using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GlucoseAPI.Models.Entities;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace GlucoseTrackerApp.Services
{
    public sealed class RestService
    {
        private string _userToken;
        public string UserToken 
        {
            get
            {
                return _userToken;
            }
            set
            {
                _userToken = value;

                if (_userToken is null)
                {
                    _client.DefaultRequestHeaders.Remove("token");
                }
                else
                {
                    _client.DefaultRequestHeaders.Add("token", _userToken);
                }

            }
        }
        private static readonly RestService _restService = new RestService();
        private readonly HttpClient _client;
        private readonly string  _baseAddress;
        private HttpResponseMessage _response;
        private string _data;
        private CancellationTokenSource _token;

        #region Constructors
        private RestService()
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromMilliseconds(7000);
            _baseAddress = "http://glucosetracker.duckdns.org:8080/api/";

        }

        public static RestService GetRestService()
        {

            if(!(_restService is null))
            {
                return _restService;
            }
            return null;
        }
        #endregion

        #region Authentication
        public async Task<string> LoginAsync(Credentials creds)
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");
                _token = new CancellationTokenSource();
                //Retrive the patients's token
                StringContent loginContent = new StringContent(JObject.FromObject(creds).ToString(), Encoding.UTF8, "application/json");
                _response = await _client.PostAsync(new Uri(_baseAddress) + "Auth/", loginContent, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();

                UserToken = _data;

                //Return the Token
                return _data;
            }
            catch (Exception)
            {
                _token.Cancel();
                return null;
            }

        }
        #endregion

        #region Registration
        public async Task<string> RegisterAsync(PatientCreationBundle patientCreationBundle)
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");
                _token = new CancellationTokenSource();
                //Serialize the patientCreationBundle and send it to the API.
                StringContent registerContent = new StringContent(JObject.FromObject(patientCreationBundle).ToString(), Encoding.UTF8, "application/json");
                _response = await _client.PostAsync(new Uri(_baseAddress + "User/Create/"), registerContent, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();

                return _data;
            }
            catch (Exception)
            {
                _token.Cancel();
                return null;
            }
        }
        #endregion

        #region Read, Update, and Delete Patient
        public async Task<Patient> ReadPatientAsync()
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");
                _token = new CancellationTokenSource();

                _response = await _client.PostAsync(new Uri(_baseAddress + "User/Read/"), null, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();

                Patient patient = JsonConvert.DeserializeObject<Patient>(_data);
                return patient;
            }
            catch (Exception)
            {
                _token.Cancel();
                return null;
            }
        }

        public async Task<string> UpdatePatientAsync(Patient patient)
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");
                _token = new CancellationTokenSource();
                //Serialize the patientCreationBundle and send it to the API.
                StringContent patientContent = new StringContent(JObject.FromObject(patient).ToString(), Encoding.UTF8, "application/json");

                await _client.PutAsync(new Uri(_baseAddress + "User/Update/"), patientContent, _token.Token);
                return "Success";
            }
            catch (Exception)
            {
                _token.Cancel();
                return "Failure";
            }
        }

        public async Task<string> DeletePatientAsync()
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");
                //Serialize the patientCreationBundle and send it to the API.
                _token = new CancellationTokenSource();

                await _client.DeleteAsync(new Uri(_baseAddress + "User/Delete/"),_token.Token);
                return "Success";
            }
            catch (Exception)
            {
                _token.Cancel();
                return "Failure";
            }
        }
        #endregion

        #region Read Doctor
        /// <summary>
        /// Reads a Doctor
        /// </summary>
        /// <returns>A Doctor</returns>
        public async Task<Doctor> ReadDoctorAsync()
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");
                _token = new CancellationTokenSource();

                _response = await _client.PostAsync(new Uri(_baseAddress + "User/Read/"), null, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();

                Doctor doctor = JsonConvert.DeserializeObject<Doctor>(_data);
                return doctor;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Create, Read, Update, and Delete Patient Data
        public async Task<string> CreatePatientData(PatientData patientData)
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");
                _token = new CancellationTokenSource();

                //Serialize the patientCreationBundle and send it to the API.
                StringContent createDataContent = new StringContent(JObject.FromObject(patientData).ToString(), Encoding.UTF8, "application/json");

                _response = await _client.PostAsync(new Uri(_baseAddress + "Data/Create/"), createDataContent, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();
                return "Success";
            }
            catch (Exception)
            {
                return "Failure";
            }
        }

        public async Task<PatientData> ReadPatientDataAsync()
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");
                _token = new CancellationTokenSource();
                _response = await _client.PostAsync(new Uri(_baseAddress + "Data/Read/"), null, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();

                PatientData patientData = JsonConvert.DeserializeObject<PatientData>(_data);
                return patientData;
            }
            catch (Exception)
            {
                _token.Cancel();
                return null;
            }
        }

        public async Task<string> UpdatePatientDataAsync(PatientData patientData)
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");

                _token = new CancellationTokenSource();
                //Serialize the patientCreationBundle and send it to the API.
                StringContent patientContent = new StringContent(JObject.FromObject(patientData).ToString(), Encoding.UTF8, "application/json");

                _response = await _client.PutAsync(new Uri(_baseAddress + "Data/Update/"), patientContent, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();
                return "Success";
            }
            catch (Exception)
            {
                return "Failure";
            }
        }

        public async Task<string> DeletePatientDataAsync(PatientData patientData)
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");
                _token = new CancellationTokenSource();
                //Serialize the patientCreationBundle and send it to the API.
                StringContent patientContent = new StringContent(JObject.FromObject(patientData).ToString(), Encoding.UTF8, "application/json");

                _response = await _client.PostAsync(new Uri(_baseAddress + "Data/Delete/"), patientContent, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();
                return "Success";
            }
            catch (Exception)
            {
                return "Failure";
            }

        }
        #endregion

        #region Query USDA
        public async Task<List<MealItem>> FindMealDataAsync(string query)
        {
            _token = new CancellationTokenSource();
            StringContent queryContent = new StringContent("{ \"generalSearchInput\": \"" + $"{query}" + "\" }", Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(new Uri("https://api.nal.usda.gov/fdc/v1/search?api_key=1l3ujGaRMh4QaOuxKEyqYSwNIDx0dSr9tmKycClk"), queryContent, _token.Token);
            _data = await _response.Content.ReadAsStringAsync();


            JObject jObject = JObject.Parse(_data);
            JArray jArray = jObject.SelectToken("foods") as JArray;

            List<MealItem> meals = new List<MealItem>(jArray.Count);

            foreach (var item in jArray)
            {
                var value = await ReadMealDataAsync((int)item.SelectToken("fdcId"));
                if (value != 0)
                {
                    meals.Add(new MealItem
                    {
                        FoodName = query,
                        Carbs = (int)value
                    });
                }
            }
            if (meals.Count > 0)
            {
                return meals;
            }
            else
            {
                return null;
            }
        }

        public async Task<float> ReadMealDataAsync(int fdcId)
        {
            _token = new CancellationTokenSource();
            _response = await _client.GetAsync(new Uri($"https://api.nal.usda.gov/fdc/v1/{fdcId}?api_key=1l3ujGaRMh4QaOuxKEyqYSwNIDx0dSr9tmKycClk"), _token.Token);
            _data = await _response.Content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(_data);

            JArray nutrients = jObject.SelectToken("foodNutrients") as JArray;

            try
            {
                foreach (var nutrient in nutrients)
                {
                    string nutrientName = (string)(nutrient.SelectToken("nutrient.name"));
                    if (nutrientName.Contains("Carbohydrate"))
                    {
                        return ((float)nutrient.SelectToken("amount"));
                    }
                }
                return 0.00F;

            }
            catch (Exception)
            {
                return 0.00F;
            }
        }
        #endregion

        #region Create and Read MealItem
        public async Task<string> CreateMealItemAsync(MealItem mealItem)
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");
                _token = new CancellationTokenSource();
                //Serialize the patientCreationBundle and send it to the API.
                StringContent mealItemContent = new StringContent(JObject.FromObject(mealItem).ToString(), Encoding.UTF8, "application/json");

                _response = await _client.PostAsync(new Uri(_baseAddress + "Meal/Create/"), mealItemContent, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();
                return _data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<MealItem> ReadMealItemAsync(string query)
        {
            try
            {
                await _client.GetAsync(new Uri(_baseAddress) + "Auth/");
                _token = new CancellationTokenSource();
                //Serialize the patientCreationBundle and send it to the API.


                var temp = $"Meal/Read?name={query.Replace(" ", "+")}";
                _response = await _client.PostAsync(new Uri(_baseAddress + $"Meal/Read?name={query.Replace(" ", "+")}"), null, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();

                MealItem mealItem = JsonConvert.DeserializeObject<MealItem>(_data);
                return mealItem;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}
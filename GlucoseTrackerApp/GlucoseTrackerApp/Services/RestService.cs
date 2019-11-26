using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GlucoseAPI.Models.Entities;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading;

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
            _client.Timeout = TimeSpan.FromMilliseconds(1000);
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
                _token = new CancellationTokenSource();
                //Retrive the patients's token
                StringContent loginContent = new StringContent(JObject.FromObject(creds).ToString(), Encoding.UTF8, "application/json");
                _response = await _client.PostAsync(new Uri(_baseAddress) + "Auth/", loginContent, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();

                _client.DefaultRequestHeaders.Add("token", _data);

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

        public async void UpdatePatientAsync(Patient patient)
        {
            try
            {
                _token = new CancellationTokenSource();
                //Serialize the patientCreationBundle and send it to the API.
                StringContent patientContent = new StringContent(JObject.FromObject(patient).ToString(), Encoding.UTF8, "application/json");

                await _client.PutAsync(new Uri(_baseAddress + "User/Update/"), patientContent, _token.Token);
            }
            catch (Exception)
            {
                _token.Cancel();
            }
        }

        public async void DeletePatientAsync()
        {
            try
            {
                //Serialize the patientCreationBundle and send it to the API.
                _token = new CancellationTokenSource();

                await _client.DeleteAsync(new Uri(_baseAddress + "User/Delete/"),_token.Token);
            }
            catch (Exception)
            {
                _token.Cancel();
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
        public async void CreatePatientData(PatientData patientData)
        {
            try
            {
                _token = new CancellationTokenSource();

                //Serialize the patientCreationBundle and send it to the API.
                StringContent createDataContent = new StringContent(JObject.FromObject(patientData).ToString(), Encoding.UTF8, "application/json");

                _response = await _client.PostAsync(new Uri(_baseAddress + "Data/Create/"), createDataContent, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                _token.Cancel();
            }
        }

        public async Task<PatientData> ReadPatientDataAsync()
        {
            try
            {
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

        public async void UpdatePatientDataAsync(PatientData patientData)
        {
            try
            {
                _token = new CancellationTokenSource();
                //Serialize the patientCreationBundle and send it to the API.
                StringContent patientContent = new StringContent(JObject.FromObject(patientData).ToString(), Encoding.UTF8, "application/json");

                _response = await _client.PutAsync(new Uri(_baseAddress + "Data/Update/"), patientContent, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return;
            }
        }

        public async void DeletePatientDataAsync(PatientData patientData)
        {
            try
            {
                _token = new CancellationTokenSource();
                //Serialize the patientCreationBundle and send it to the API.
                StringContent patientContent = new StringContent(JObject.FromObject(patientData).ToString(), Encoding.UTF8, "application/json");

                _response = await _client.PostAsync(new Uri(_baseAddress + "Data/Delete/"), patientContent, _token.Token);
                _data = await _response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return;
            }

        }
        #endregion

        #region Query USDA
        public async Task<int> FindMealDataAsync(string query)
        {
            _token = new CancellationTokenSource();
            StringContent queryContent = new StringContent("{ \"generalSearchInput\": \"" + $"{query}" + "\" }", Encoding.UTF8, "application/json");
            _token.CancelAfter(TimeSpan.FromMilliseconds(7000));
            _response = await _client.PostAsync(new Uri("https://api.nal.usda.gov/fdc/v1/search?api_key=1l3ujGaRMh4QaOuxKEyqYSwNIDx0dSr9tmKycClk"), queryContent, _token.Token);
            _data = await _response.Content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(_data);

            int fdcId = (int) jObject.SelectToken("foods[0].fdcId");
            return fdcId;
        }

        public async Task<float> ReadMealDataAsync(int fdcId)
        {
            _token = new CancellationTokenSource();
            _token.CancelAfter(TimeSpan.FromMilliseconds(7000));
            _response = await _client.GetAsync(new Uri($"https://api.nal.usda.gov/fdc/v1/{fdcId}?api_key=1l3ujGaRMh4QaOuxKEyqYSwNIDx0dSr9tmKycClk"), _token.Token);
            _data = await _response.Content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(_data);

            JArray nutrients = jObject.SelectToken("foodNutrients") as JArray;

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
        #endregion

        #region Create and Read MealItem
        public async Task<string> CreateMealItemAsync(MealItem mealItem)
        {
            try
            {
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
                _token = new CancellationTokenSource();
                //Serialize the patientCreationBundle and send it to the API.

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
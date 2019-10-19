﻿using System;
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

        public async Task<string> LoginAsync(Credentials creds)
        {
            //Retrive the patients's token
            StringContent loginContent = new StringContent(JObject.FromObject(creds).ToString(), Encoding.UTF8, "application/json");
            _response =  await _client.PostAsync(new Uri(_baseAddress) + "Token/", loginContent);
            _data = await _response.Content.ReadAsStringAsync();

            //Return the Token
            return _data;
        }

        public async void RegisterAsync(PatientCreationBundle patientCreationBundle)
        {
            //Serialize the patientCreationBundle and send it to the API.

            StringContent registerContent = new StringContent(JObject.FromObject(patientCreationBundle).ToString(), Encoding.UTF8, "application/json");
            _response = await _client.PostAsync(new Uri(_baseAddress + "Create/"), registerContent);
            _data = await _response.Content.ReadAsStringAsync();
        }
    }
}
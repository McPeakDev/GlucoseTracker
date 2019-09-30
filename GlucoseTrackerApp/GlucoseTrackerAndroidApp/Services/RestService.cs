using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using GlucoseTrackerWeb.Models;
using Newtonsoft.Json;
using GlucoseAPI.Models.DBFEntities;

namespace GlucoseTrackerAndroidApp.Services
{
    public class RestService
    {
        HttpClient _client;
        string  _baseAddress;
        HttpResponseMessage _response;
        string _data;

        public RestService()
        {
            _client = new HttpClient();
            _baseAddress = "http://glucosetracker.duckdns.org:8080/";

        }

        public async Task<User> LoginAsync(Credentials creds)
        {
            StringContent loginContent = new StringContent(creds.ToString(), Encoding.UTF8, "application/json");
            _response =  await _client.PostAsync(new Uri(_baseAddress), loginContent);
            _data = await _response.Content.ReadAsStringAsync();
            User user = JsonConvert.DeserializeObject<User>(_data.ToString());
            return user;
        } 
    }
}
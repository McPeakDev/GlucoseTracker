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

namespace GlucoseTrackerAndroidApp.Services
{
    public class RestService
    {
        HttpClient _client;

        public RestService()
        {
            _client = new HttpClient();
        }

        public async Task<Boolean> LoginAsync(Credentials creds)
        {
            User user = null;
            var uri = new Uri(string.Format($"http://localhost:5000/API/$2y$12$KPxgNtzXN1PvL89l7nrnJ.MPFxLfCz7BZvI8fa5lyORGNw5S.O9x./{creds.Email}/{creds.Password}"));

            var response = await _client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                //var content = response.Content.ReadAsStringAsync();
                return true;
                //user = JsonConvert.DeserializeObject<User>(content);
            }
            return false;
        } 
    }
}
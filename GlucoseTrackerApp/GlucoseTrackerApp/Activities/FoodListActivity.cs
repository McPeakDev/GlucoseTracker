///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerApp/GlucoseTrackerApp
//	File Name:         BloodSugarAddActivity.cs
//	Description:       Methods for adding blood sugar readings for the mobile app
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


using System;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using GlucoseAPI.Models.Entities;
using GlucoseTrackerApp.Services;
using Android.Widget;
using Android.Content;
using System.Threading.Tasks;
using System.Linq;
using static Android.Widget.AdapterView;

namespace GlucoseTrackerApp
{
    [Activity(Label = "Add A Blood Sugar Reading", Theme = "@style/Theme.MaterialComponents.Light.NoActionBar")]
    public class FoodListActivity : AppCompatActivity
    {
        private readonly RestService _restService = RestService.GetRestService();
        private ListView _mealList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_list);

            _mealList = FindViewById<ListView>(Resource.Id.meal_list);

            _mealList.ItemClick += (object sender, ItemClickEventArgs e) => 
            {

                var meal = _mealList.GetItemAtPosition(e.Position).Cast<MealItem>();

                if (Intent.GetBooleanExtra("BloodSugar", false))
                {

                    Intent carbActivity = new Intent(this, typeof(BloodSugarAddWithFoodActivity));
                    carbActivity.PutExtra("MealName", meal.FoodName);
                    carbActivity.PutExtra("Carbs", meal.Carbs.ToString());
                    StartActivity(carbActivity);
                    Finish();
                }
                else
                {
                    var alert = new Android.App.AlertDialog.Builder(this);

                    alert.SetTitle("Alert");
                    alert.SetMessage("Do you want to add a blood sugar with this?");
                    alert.SetPositiveButton("Yes", (c, ev) =>
                    {
                        alert.Dispose();

                        Intent carbActivity = new Intent(this, typeof(BloodSugarAddWithFoodActivity));
                        carbActivity.PutExtra("MealName", meal.FoodName);
                        carbActivity.PutExtra("Carbs", meal.Carbs.ToString());
                        StartActivity(carbActivity);
                        Finish();
                    });

                    alert.SetNegativeButton("No", (c, ev) =>
                    {
                        alert.Dispose();

                        Intent carbActivity = new Intent(this, typeof(CarbAddActivity));
                        carbActivity.PutExtra("MealName", meal.FoodName);
                        carbActivity.PutExtra("Carbs", meal.Carbs.ToString());
                        StartActivity(carbActivity);
                        Finish();
                    });

                    alert.Show();
                }
                
            };

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar_list);
            toolbar.Title = "Pick a Meal";
            SetSupportActionBar(toolbar);

        }

        protected async override void OnStart()
        {
            base.OnStart();
            var list = await _restService.FindMealDataAsync(Intent.GetStringExtra("MealName"));
            ArrayAdapter<MealItem> adapter = new ArrayAdapter<MealItem>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, list);
            _mealList.Adapter = adapter;
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            Intent loginActivity = new Intent(this, typeof(LoginActivity));
            StartActivity(loginActivity);
            Finish();
        }
    }
}
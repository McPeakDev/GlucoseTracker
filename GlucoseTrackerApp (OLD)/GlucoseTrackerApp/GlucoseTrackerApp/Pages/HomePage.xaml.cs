using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlucoseTrackerApp.ObjectClasses;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GlucoseTrackerApp.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
		
		Patient thisPatient = null;
		public HomePage(Patient patient)
		{

		    Title = "Glucose Tracker Homepage";
			var HomeLayout = new StackLayout();
			Label welcomeLabel = new Label
			{
				Text = "Welcome to the Homepage!: " + patient.FirstName
			};

			HomeLayout.Children.Add(welcomeLabel);
            this.Content = HomeLayout;
		}
		
	}
}
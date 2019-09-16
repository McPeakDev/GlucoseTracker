using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GlucoseTrackerApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            
            Title = "Glucose Tracker Login";
            var LoginLayout = new StackLayout();
            LoginLayout.Children.Add(new Entry { Placeholder = "Username", HorizontalOptions = LayoutOptions.Fill });
            LoginLayout.Children.Add(new Entry { IsPassword = true, Placeholder = "Password", HorizontalOptions = LayoutOptions.Fill });
            LoginLayout.Children.Add(new Button
            {
                Text = "Log In",
                TextColor = Color.White,
                BackgroundColor = Color.Gray,
                HorizontalOptions = LayoutOptions.Fill
            });
            LoginLayout.Children.Add(new Button
            {
                Text = "Create Account",
                TextColor = Color.Blue,
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.Fill
            });
            //layout.Children.Add(new Frame { Content = modernLayout });
            this.Content = LoginLayout;
        }

        //async void OnLoginButtonClicked(object sender, EventArgs e)
        //{
        //    await Navigation.PopAsync();
        //}
        //async void OnCreateButtonClicked(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new AccountCreation());
        //}
    }
}
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
    public partial class LoginPage : ContentPage
    {
        string userName = "";
        string password = "";
        Entry UserName = null;
        Entry Password = null;
        public LoginPage()
        {
           
            
            Title = "Glucose Tracker Login";
            var LoginLayout = new StackLayout();
            UserName = new Entry { Placeholder = "Username", HorizontalOptions = LayoutOptions.Fill, };
            Password = new Entry { IsPassword = true, Placeholder = "Password", HorizontalOptions = LayoutOptions.Fill };
            LoginLayout.Children.Add(UserName);
            LoginLayout.Children.Add(Password);
            Button LoginButton = new Button
            {

                Text = "Log In",
                TextColor = Color.White,
                BackgroundColor = Color.Gray,
                HorizontalOptions = LayoutOptions.Fill
            };

            LoginButton.Clicked += OnLoginButtonClicked;
                
            Button CreateAccount = new Button
            {

                Text = "Create Account",
                TextColor = Color.Blue,
                BackgroundColor = Color.White,
                HorizontalOptions = LayoutOptions.Fill
            };

            CreateAccount.Clicked += OnCreateButtonClicked;

            LoginLayout.Children.Add(LoginButton);
            LoginLayout.Children.Add(CreateAccount);
            //layout.Children.Add(new Frame { Content = modernLayout });
            
            this.Content = LoginLayout;
        }

        async void OnLoginButtonClicked(object sender, EventArgs e )
        {
            if(UserName.Text != null && Password.Text != null)
            {
                userName = UserName.Text;
                password = Password.Text;
                Patient patient = new Patient(1, "me", "zach", "J", "test@etsu.edu", "4235559000");
                HomePage homePage = new HomePage(patient);
                
                await Navigation.PushAsync(homePage);
            }

            //await Navigation.PushAsync(new HomePage());
        }
        async void OnCreateButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccountCreation());
        }
    }
}
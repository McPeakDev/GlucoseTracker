using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GluecoseTrackerApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
           
        }


        
    }

    public class CodeButtonClickPage : ContentPage
    {
        public CodeButtonClickPage()
        {
            Title = "Code Button Click";

            Label label = new Label
            {
                Text = "Click the Button below",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center
            };

            Button button = new Button
            {
                Text = "Click to Rotate Text!",
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.Center
            };
            button.Clicked += async (sender, args) => await label.RelRotateTo(360, 1000);

            Content = new StackLayout
            {
                Children =
            {
                label,
                button
            }
            };
        }
    }

}

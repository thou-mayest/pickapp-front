using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace pikappDes
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
          
            Preferences.Set("FREE", true);
            
            if(Preferences.ContainsKey("L"))
            {
                if(Preferences.Get("L","") == "T")
                {
                    NavigationPage master = new NavigationPage(new NewMainMaster());


                    Application.Current.MainPage = master;
                }
            }
            if(!Preferences.ContainsKey("USR"))
                Preferences.Set("USR", "C");
            Navigation.PushAsync(new DetailPage(),true);
        }

        private void ContactButton_Clicked(object sender, EventArgs e)
        {

            Navigation.PushAsync(new ProAccPage(), true);

        }
    }
}

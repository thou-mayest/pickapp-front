using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pikappDes.Utils;
using pikappDes.Utils.modals;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void LogInButt_Clicked(object sender, EventArgs e)
        {

            var LoadingPage = new LoadingPage();
            await Navigation.PushModalAsync(LoadingPage, true);

            Creds creds = new Creds
            {
                phone = Phone.Text,
                pass = Password.Text
            };

            string uri = await Utility.GetUri();
            string res = await Utility.LoginReq(uri, creds, "0");

            while (this.Navigation.ModalStack.Count > 0)
            {
                await this.Navigation.PopModalAsync();
            }

            if (res != null && res != string.Empty)
            {
                Creds resCreds = JsonConvert.DeserializeObject<Creds>(res);

                
                Preferences.Set("UID", resCreds.UID);
                Preferences.Set("SID", resCreds.SID);
                //usrname = Preferences.Get("NAME", "No Val Error");
                //number = Preferences.Get("NUMBER", "No Val Error");

                Preferences.Set("NAME", resCreds.name);
                Preferences.Set("NUMBER", Phone.Text);

                Preferences.Set("T", resCreds.type.ToString());
                Preferences.Set("L", true);

               
                NavigationPage master = new NavigationPage(new NewMainMaster());


                Application.Current.MainPage = master;

            }
            else
            {
                await DisplayAlert("ERROR", "LOGIN FAILED ..SIGN UP ?", "Ok");
            }

        }

        private void Signup_clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new DetailPage(), true);
        }
    }
}
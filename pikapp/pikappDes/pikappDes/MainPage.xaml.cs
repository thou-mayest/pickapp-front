using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using pikappDes.Utils;
using pikappDes.Utils.modals;

namespace pikappDes
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        string uri;

        public MainPage()
        {

            InitializeComponent();
            GetUri();
        }

        
        public async Task GetUri()
        {
            uri = await Utility.GetUri();
        }


        public async void Login_Clicked(object sender, EventArgs e)
        {

            //_Chathub = DependencyService.Get<IChat>();

            //if(!_Chathub.IsConnected())
            //{
            //    await _Chathub.Connect();
            //}



            //Creds auth = new Creds
            //{
            //    UID = "UID AUTH FOR CHAT:  " + DateTime.Now,
            //};

            //await _Chathub.Register(auth);


            //await DisplayAlert("titel", "message test", "cancel");
            if (Preferences.Get("L", false))
            {
                NavigationPage master = new NavigationPage(new NewMainMaster());


                Application.Current.MainPage = master;

            }
            else
            {
                var LoadingPage = new LoadingPage();
                await Navigation.PushModalAsync(LoadingPage, true);



                Enum.TryParse(Preferences.Get("T", "Client"), out ClienType type);

                Creds creds = new Creds
                {
                    UID = Preferences.Get("UID", ""),
                    SID = Preferences.Get("SID", ""),
                    type = type
                };



                string uri = await Utility.GetUri();
                string res = await Utility.LoginReq(uri, creds, "1");

                while (this.Navigation.ModalStack.Count > 0)
                {
                    await this.Navigation.PopModalAsync();
                }

                if (res == "ERROR")
                {

                    await DisplayAlert("Error", "NETWORK ERROR", "Cancel");
                }
                else if (res != null && res != string.Empty && res != "false")
                {

                    NavigationPage master = new NavigationPage(new NewMainMaster());
                    Application.Current.MainPage = master;
                }
                else
                {
                    NavigationPage master = new NavigationPage(new Login());
                    Application.Current.MainPage = master;

                }
            }



            /////////////////////////////////////////////////////////:::
            ///                             OLD
            ///                             ////////////////////////////

            //Preferences.Set("FREE", true);

            //if(Preferences.ContainsKey("L"))
            //{
            //    if(Preferences.Get("L","") == "T")
            //    {
            //        NavigationPage master = new NavigationPage(new NewMainMaster());


            //        Application.Current.MainPage = master;
            //    }
            //}
            //if(!Preferences.ContainsKey("USR"))
            //    Preferences.Set("USR", "C");
            //Navigation.PushAsync(new DetailPage(),true);
        }

        private void ContactButton_Clicked(object sender, EventArgs e)
        {

            Navigation.PushAsync(new ProAccPage(), true);

        }
    }
}

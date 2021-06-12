using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using Xamarin.Essentials;

namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DataEntryPage : ContentPage
    {
        Uri uri;

        public DataEntryPage()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            GetUri();
        }
        static HttpClient client = new HttpClient();
        private async void GetUri()
        {
            LogInButt.IsEnabled = false;
            LogInButt.Text = "Connecting to service...";
            Name.Text = Preferences.Get("NAME","");
            Phone.Text = Preferences.Get("NUMBER", "");
            Password.Text = Preferences.Get("PASS", "");
            try
            {
                //uri = new Uri(await client.GetStringAsync(furi) + "/api/values"); 

                uri = new Uri(await Utility.GetUri());
                
                //uri = await Utility.GetUri();
                if(uri != null)
                {
                    LogInButt.Text = "Login";
                    LogInButt.IsEnabled = true;
                    //await DisplayAlert("FINE", uri.ToString(), "Ok"); 
                }
                else
                {
                    await DisplayAlert("ERROR", "Check internet connection", "Ok");
                }
               
            }
            catch (Exception)
            {
               
               
            }
            
           
        }
        
        private async void Button_Clicked(object sender, EventArgs e)
        {
            if(Name.Text != "" & int.TryParse(Phone.Text,out int n) & Phone.Text != "")
            {

                var LoadingPage = new LoadingPage();
                await Navigation.PushModalAsync(LoadingPage, true);

              

                TaxisProp item = new TaxisProp
                {
                    name = Name.Text,
                    phone = int.Parse(Phone.Text),
                    //pos = FamName.Text,
                    pass = Password.Text
                };

                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Add("REQ_TYPE", "SIGN_UP");
                client.DefaultRequestHeaders.Add("REQ_USR", GetUSR());

                try
                {
                    HttpResponseMessage response = await client.PostAsync(uri, content);

                    if (response.Content.ReadAsStringAsync().Result == "USER_ADDED" || response.Content.ReadAsStringAsync().Result == "LOGIN_S")
                    {
                        Preferences.Set("L", "T");

                        // save entry as it's correct
                        Preferences.Set("NAME", Name.Text);
                        Preferences.Set("NUMBER", Phone.Text);
                        Preferences.Set("PASS", Password.Text);

                        while (this.Navigation.ModalStack.Count > 0)
                        {
                            await this.Navigation.PopModalAsync();
                        }
                        LogInButt.IsEnabled = false;

                        NavigationPage master = new NavigationPage(new NewMainMaster());


                        Application.Current.MainPage = master;
                    }
                    else if(response.Content.ReadAsStringAsync().Result == "wrong password")
                    {
                        while (this.Navigation.ModalStack.Count > 0)
                        {
                            await this.Navigation.PopModalAsync();
                        }

                        await DisplayAlert("ERROR", "Wrong password", "Ok");
                        
                    }
                    else
                    {
                        while (this.Navigation.ModalStack.Count > 0)
                        {
                            await this.Navigation.PopModalAsync();
                        }
                        //THIIS IS THE ORGINAL WORK
                        //await DisplayAlert("ERROR", response.Content.ReadAsStringAsync().Result, "OK");

                        await DisplayAlert("ERROR", "server down try again later", "Ok");

                    }

                }
                catch (Exception x)
                {
                    await DisplayAlert("ERROR", "error has occured : " + x.Message, "OK");
                    while (this.Navigation.ModalStack.Count > 0)
                    {
                        await this.Navigation.PopModalAsync();
                    }
                    //THIIS IS THE ORGINAL WORK
                    //await DisplayAlert("ERROR", response.Content.ReadAsStringAsync().Result, "OK");

                    NavigationPage master = new NavigationPage(new NewMainMaster());


                    //Application.Current.MainPage = master;
                    client.CancelPendingRequests();

                }



                client.DefaultRequestHeaders.Clear();


            }
            else
                await DisplayAlert("ERROR", "VERIFY INFO ", "OK");


        }

        public string GetUSR()
        {
            return Preferences.Get("USR", "C");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using Newtonsoft.Json;
using pikappDes;
using Plugin.Geolocator;

namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewMainMaster : FlyoutPage
    {
        Dictionary<int, NavigationPage> pages = new Dictionary<int, NavigationPage>();
        public NewMainMaster()
        {
            InitializeComponent();
            MasterPage.MenuListViw.ItemSelected += ListView_ItemSelected;

            pages.Add(0, new NavigationPage(new NewMainMasterDetail()));
            pages.Add(1, new NavigationPage(new MessagesListPage()));
            pages.Add(2, new NavigationPage(new OptionsPage()));
            pages.Add(3, new NavigationPage(new LoadingPage()));
            pages.Add(4, new NavigationPage(new ProAccPage()));

            NavigationPage navpage;
            pages.TryGetValue(0, out navpage);

            navpage.Title = "Map";


            Detail = navpage;
            Detail.Title = "Map";

            IsPresented = false;
        }

        public void changeView(int id)
        {

        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as NewMainMasterMasterMenuItem;
            if (item == null)
                return;
            if(item.Id == 5)
            {
                Preferences.Set("L", false);
                Preferences.Set("UID", "");
                Preferences.Set("SID", "");
                Preferences.Set("NAME", "");
                Preferences.Set("NUMBER", "");

                IsPresented = false;


                //pages.Clear();

                //Navigation.PopToRootAsync();
                //NavigationPage master = new NavigationPage(new MainPage());

                //KillMe();

                NavigationPage master = new NavigationPage(new MainPage());
                Application.Current.MainPage = master;

                //Application.Current.MainPage = master;
                return;
                
            }

            // ===================== original

            //var page = (Page)Activator.CreateInstance(item.TargetType);
            //page.Title = item.Title;

            //Detail = new NavigationPage(page);
            //IsPresented = false;

            //MasterPage.MenuListViw.SelectedItem = null;

            // ============================== NEW

            NavigationPage navpage;

            pages.TryGetValue(item.Id, out navpage);
            navpage.Title = item.Title;

            Detail = navpage;
            Detail.Title = item.Title;

            IsPresented = false;

            MasterPage.MenuListViw.SelectedItem = null;
        }
        public async Task KillMe()
        {
            await Task.Delay(1000);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        private void msg_button_Clicked(object sender, EventArgs e)
        {
            //Detail = new NavigationPage(new MessagesPage());

            NavigationPage navpage;

            pages.TryGetValue(1, out navpage);
            navpage.Title = "Messages";


            Detail = navpage;
            Detail.Title = "Messages";
        }

        public bool free;
        //public HttpClient client = new HttpClient();

        //private void Free_Clicked(object sender, EventArgs e)
        //{
        //    Free_button.IconImageSource = "pin.png";
        //    if (Preferences.ContainsKey("FREE"))
        //        free = Preferences.Get("FREE", true );


        //    switch (free)
        //    {
        //        case true:
        //            Preferences.Set("FREE", false);
        //            Free_button.IconImageSource = "OffToggle.png";
        //            break;
        //        case false:
        //            Preferences.Set("FREE",true);
        //            Free_button.IconImageSource = "OnToggle.png";
        //            break;
        //    }
        //}

        public void SendNotFReq()
        {

            //var posGPS = Utility.GetPos();

            //TaxisProp UpdateItem = new TaxisProp
            //{
            //    name = Preferences.Get("NAME", ""),
            //    phone = int.Parse(Preferences.Get("NUMBER", "")),
            //    //pos = posGPS.Latitude.ToString() + "/" + posGPS.Longitude.ToString(),
            //    pass = Preferences.Get("PASS", ""),
            //    free = Preferences.Get("FREE", true)
            //};

            //var json = JsonConvert.SerializeObject(UpdateItem);
            //var UpdateContent = new StringContent(json, Encoding.UTF8, "application/json");
            //// var content = new StringContent(json, Encoding.UTF8, "application/json");
            //client.DefaultRequestHeaders.Clear();
            //client.DefaultRequestHeaders.Add("REQ_TYPE", "UPDATE_pos");
            //client.DefaultRequestHeaders.Add("REQ_USR", Preferences.Get("USR", "C"));

            //try
            //{
            //    var response = client.PostAsync(Uri, UpdateContent);
            //}
            //catch (Exception)
            //{
            //    await GetUriAsync();
            //}

        }

    }
}
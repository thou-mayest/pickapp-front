using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using Xamarin.Forms.Maps;
using System.Net.Http;
using Xamarin.Essentials;
using pikappDes.Utils;
using pikappDes.Utils.modals;



namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewMainMasterDetail : ContentPage
    {

        Creds Mycreds = new Creds();
        /// <summary>
        ///                 THIS IS MAIN MAP LOGIC
        /// </summary>
        public NewMainMasterDetail()
        {
            InitializeComponent();
            Enum.TryParse(Preferences.Get("T", "Client"), out ClienType type);
            Mycreds.UID = Preferences.Get("UID", "");
            Mycreds.SID = Preferences.Get("SID", "");
            Mycreds.type = type;
           
            updateMap();
        }

        static HttpClient client = new HttpClient();

        
        static Uri Uri;
        List<UserProp> UserList = new List<UserProp>();

        public async Task GetUriAsync()
        {
            Uri = new Uri(await Utility.GetUri());
        }

        bool firstRun = true;
        
        private async Task updateMap()
        {

            if (firstRun)
            {
                firstRun = false;

                MyMap.IsVisible = false;
            }
            var LocatorStatus =await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
               
            if (LocatorStatus != PermissionStatus.Granted)
            {
                LocatorStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                await Task.Delay(500);
                await updateMap();
            }
            else
            {
                try
                {
                    await GetUriAsync();

                    var posGPS = await Utility.GetPos();

                    // get position and update URI (it's public var) 
                    

                    MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(posGPS.Latitude, posGPS.Longitude), Distance.FromKilometers(0.3)));

                    MyMap.IsVisible = true;
                    MyMap.IsShowingUser = true;

                    MyMap.CustomPins = new List<CustomPins>();

                    await UpdateLists();
                }
                catch (Exception)
                {
                    await DisplayAlert("error", "balizz open geolocation ", "OK");
                }
                await updateMap();
            }

        }

        Random rn = new Random();

         
        private async Task UpdateLists()
        {


            if (MyMap.MapType == MapType.Hybrid && Preferences.Get("MAP", "H") != "H")
            {
                MyMap.MapType = MapType.Street;
            }
            else if (MyMap.MapType == MapType.Street && Preferences.Get("MAP","H") != "S")
            {
                MyMap.MapType = MapType.Hybrid ;
            }
            //updating taxis list

            //string uri = "https://00c83087.ngrok.io/api/values";
            if (Uri != null)
            {
                    //client.DefaultRequestHeaders.Clear();
                //client.DefaultRequestHeaders.Add("USR", Preferences.Get("USR","C"));
                try
                {
                    UserList.Clear();


                    string res = await Utility.GetList(Uri.ToString(), Mycreds);

                    if (res != "ERROR")
                    {
                        UserList = JsonConvert.DeserializeObject<List<UserProp>>(res);


                        MyMap.Pins.Clear();
                        if (Preferences.Get("T", "") == "Client")
                            PopulateCustomTaxi(); //=========================== populate map with pins GENERIC
                        else
                            PopulateCustomClient(); //============== populate map with pins OTHER

                    }

                }
                catch (Exception)
                {
                    await GetUriAsync();

                }

                await SendUpdate();
            }
            else
            {
                await DisplayAlert("ERROR3", "could not connect to server ", "OK");
            }

            await Task.Delay(6000);

            await UpdateLists();
        }

        public void PopulateCustomTaxi()
        {
            foreach (UserProp item in UserList)
            {
                CustomPins pin = new CustomPins
                {
                    name = item.name,
                    Label = item.name,
                    Address = item.pos,

                    Type = PinType.Generic,

                    phone = item.phone,
                    Position = new Position(Convert.ToDouble(item.pos.Split('/')[0]), Convert.ToDouble(item.pos.Split('/')[1]))

                };


               
                //await DisplayAlert(pin.Name, pin.Pos, "OK");

                pin.InfoWindowClicked += pin_clicked;



                MyMap.CustomPins.Add(pin);
                MyMap.Pins.Add(pin);


            }
        }

        public void PopulateCustomClient()
        {

            foreach (UserProp item in UserList)
            {
                CustomPins pin = new CustomPins
                {
                    name = item.name,
                    Label = item.name,
                    Address = item.pos,

                    Type = PinType.Place,

                    phone = item.phone,
                    Position = new Position(Convert.ToDouble(item.pos.Split('/')[0].Replace('.',',')), Convert.ToDouble(item.pos.Split('/')[1].Replace('.',','))) //change according to culture . Or ,

                };


               

                pin.InfoWindowClicked += pin_clicked;



                MyMap.CustomPins.Add(pin);
                MyMap.Pins.Add(pin);


            }
        }

        public async Task SendUpdate()
        {
            var posGPS = await Utility.GetPos();

            Enum.TryParse(Preferences.Get("T", "Client"), out ClienType type);
            
            UserProp UpdateItem = new UserProp
            {
                UID = Mycreds.UID,
                pos = posGPS.Latitude.ToString() + "/" + posGPS.Longitude.ToString(),
                free = Preferences.Get("FREE", true),
                type = type,
            };

            
            string res = await Utility.UpdatePos(Uri.ToString(), UpdateItem);
            if (res == "ERROR")
            {
                await GetUriAsync();
            }
            if(res == "LOGIN_ERROR")
            {
                await DisplayAlert("Error", "login expired plz disconnect and reconnect", "Cancel");
            }
        }

        private void pin_clicked(object sender, EventArgs e)
        {
            CustomPins selected_pin = (CustomPins)sender;

            //DisplayAlert("Number Copied to clipboard", selected_pin.Phone.ToString(), "OK");

            //PhoneDialer.Open(selected_pin.Phone.ToString());
            Launcher.OpenAsync(new Uri("tel:"+selected_pin.phone.ToString()));
        }
    }
}
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
using pikappDes;


namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewMainMasterDetail : ContentPage
    {


        /// <summary>
        ///                 THIS IS MAIN MAP LOGIC
        /// </summary>
        public NewMainMasterDetail()
        {
            InitializeComponent();
            updateMap();
        }

        static HttpClient client = new HttpClient();

        
        static Uri Uri;
        List<TaxisProp> taxis = new List<TaxisProp>();

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
                updateMap();
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
                updateMap();
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
                    client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("USR", Preferences.Get("USR","C"));
                try
                {
                    var respond = await client.GetAsync(Uri.ToString());

                    if (respond.IsSuccessStatusCode)
                    {

                        taxis.Clear();
                        var content = await respond.Content.ReadAsStringAsync();

                        taxis = JsonConvert.DeserializeObject<List<TaxisProp>>(content);

                        
                        MyMap.Pins.Clear();

                        
                        if (Preferences.Get("USR", "C") == "C")
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
            foreach (TaxisProp item in taxis)
            {
                CustomPins pin = new CustomPins
                {
                    Name = item.name,
                    Label = item.name,
                    Address = item.pos,

                    Type = PinType.Generic,

                    Phone = item.phone,
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

            foreach (TaxisProp item in taxis)
            {
                CustomPins pin = new CustomPins
                {
                    Name = item.name,
                    Label = item.name,
                    Address = item.pos,

                    Type = PinType.Place,

                    Phone = item.phone,
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

            TaxisProp UpdateItem = new TaxisProp
            {
                name = Preferences.Get("NAME", ""),
                phone = int.Parse(Preferences.Get("NUMBER", "")),
                pos = posGPS.Latitude.ToString() + "/" + posGPS.Longitude.ToString(),
                pass = Preferences.Get("PASS", ""),
                free = Preferences.Get("FREE", true)
            };

            var json = JsonConvert.SerializeObject(UpdateItem);
            var UpdateContent = new StringContent(json, Encoding.UTF8, "application/json");
            // var content = new StringContent(json, Encoding.UTF8, "application/json");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("REQ_TYPE", "UPDATE_pos");
            client.DefaultRequestHeaders.Add("REQ_USR", Preferences.Get("USR", "C"));

            

            try
            {
                var response = client.PostAsync(Uri, UpdateContent);
            }
            catch (Exception)
            {
                await GetUriAsync();
            }

        }

        private void pin_clicked(object sender, EventArgs e)
        {
            CustomPins selected_pin = (CustomPins)sender;

            //DisplayAlert("Number Copied to clipboard", selected_pin.Phone.ToString(), "OK");

            //PhoneDialer.Open(selected_pin.Phone.ToString());
            Launcher.OpenAsync(new Uri("tel:"+selected_pin.Phone.ToString()));
        }
    }
}
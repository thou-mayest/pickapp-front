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
using Rg.Plugins.Popup.Extensions;


namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewMainMasterDetail : ContentPage
    {

        Creds Mycreds = new Creds();
        /// <summary>
        ///                 THIS IS MAIN MAP LOGIC
        ///                 (put this in a singleton ) <===================
        /// </summary>
        /// 

        IChat _chat;
        public NewMainMasterDetail()
        {

            _chat = DependencyService.Get<IChat>();

            InitializeComponent();
            Enum.TryParse(Preferences.Get("T", "Client"), out ClienType type);
            Mycreds.UID = Preferences.Get("UID", "");
            Mycreds.SID = Preferences.Get("SID", "");
            Mycreds.type = type;

            _chat.OnError(OnChatError);
            _chat.OnPing(OnPing);
            _chat.OnAccepted(OnAccepted);
            _chat.RoomCreated(RoomCreate);
            TryChatConnect();
            updateMap();
            //Task.Run(() => updateMap());
            //t.Wait();

        }

        public void TryChatConnect()
        {
            bool connected;
            try
            {
                connected = _chat.IsConnected();
            }
            catch (Exception)
            {

                connected = false;
            }
            if (!connected)
            {
                try
                {
                    _chat.Connect();
                    _chat.Register(Mycreds);
                }
                catch (Exception)
                {
                    DisplayAlert("Error", "Chat service Connection failed", "Cancel");
                }


            }
        }

        static HttpClient client = new HttpClient();

        
        static Uri Uri;
        List<UserProp> UserList = new List<UserProp>();

        public async Task GetUriAsync(bool reload)
        {
            
            Uri = new Uri(await Utility.GetUri(reload));
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
                    await GetUriAsync(false);
                   
                    var posGPS = await Utility.GetPos();
                   
                    // get position and update URI 


                    MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(posGPS.Latitude, posGPS.Longitude), Distance.FromKilometers(0.3)));
                   
                    MyMap.IsVisible = true;
                    MyMap.IsShowingUser = true;

                    MyMap.CustomPins = new List<CustomPins>();
                    
                    UpdateLists();
                   
                }
                catch (Exception ex)
                {
                    await Task.Delay(2000);
                    //await DisplayAlert("error", "balizz open geolocation ", "OK");
                    await DisplayAlert("error", ex.Message, "OK");
                    await updateMap();
                }


            }

        }

         
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


                    string res = await Utility.GetList(Uri.ToString(), Mycreds); // PERFORMANCE HIT

                    if (res != "ERROR")
                    {
                        UserList = JsonConvert.DeserializeObject<List<UserProp>>(res);


                        MyMap.Pins.Clear();
                        if (Preferences.Get("T", "") == "Client")
                            PopulateCustomTaxi(); //=========================== populate map with pins GENERIC
                        else
                            PopulateCustomClient(); //============== populate map with pins OTHER

                    }
                    if(res == "ERROR")
                    {
                        await DisplayAlert("ERROR", "Connection failed", "Cancel");
                    }

                }
                catch (Exception)
                {
                    await GetUriAsync(true);

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

                    UID = item.UID,
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
                    UID = item.UID,
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
                name = Preferences.Get("NAME", "No Val Error"),
                UID = Mycreds.UID,
                pos = posGPS.Latitude.ToString() + "/" + posGPS.Longitude.ToString(),
                free = Preferences.Get("FREE", true),
                type = type,
            };

            
            string res = await Utility.UpdatePos(Uri.ToString(), UpdateItem);
            if (res == "ERROR")
            {
                await GetUriAsync(true);
            }
            if(res == "LOGIN_ERROR")
            {
                await DisplayAlert("Error", "login expired plz disconnect and reconnect", "Cancel");
            }
        }

        Random rn = new Random();
        
        private void pin_clicked(object sender, EventArgs e)
        {
            CustomPins selected_pin = (CustomPins)sender;

            TryChatConnect();

            int secret = rn.Next(1, 9999);

            _chat.AddPendingReq(secret, selected_pin.UID);

            try
            {
                
                _chat.SendPing(selected_pin.UID, Mycreds,secret);
                DisplayAlert("Send", "Ping sent: "+ selected_pin.UID, "Ok");
            }
            catch (Exception)
            {
                TryChatConnect();
            }

            

            //PhoneDialer.Open(selected_pin.Phone.ToString());
            //Launcher.OpenAsync(new Uri("tel:"+selected_pin.phone.ToString()));
        }

        private void OnChatError(string msg)
        {
            DisplayAlert("Error", msg, "Cancel");
        }

        private void OnPing(string uid,int secret)
        {
            //DisplayAlert("Ping", fromserv, "Ok");

            var popup = new PingPage(uid,secret,Mycreds);

            App.Current.MainPage.Navigation.PushPopupAsync(popup, true);

        }

        private void OnAccepted(string RID,string uid, int secret)
        {
            //if secret existes in dictionnary then call CreateRoom()
            if (_chat.IsPendingSecret(uid,secret))
            {
                DisplayAlert("Accepted", uid, "Ok");
                _chat.CreateRoom(Mycreds,RID,uid);
            }
            else
            {
                DisplayAlert("ERROR", "SECRET NOT MATCHING", "OK"); //// FOR TEST ONLY /// PROB CHALLENGE??
            }

        }

        private void RoomCreate(string RID)
        {
            // msglist += RID
            // navigation = MsgPage

            DisplayAlert("CREATED", "room: " + RID, "Ok"); // DELETE, THIS FOR TESTING ONLY

        }

    }
}
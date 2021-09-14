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
            _chat.ReRegister(TryChatConnect);
            
           
            //updateMap();

            //Task.Run(() => updateMap());
            //t.Wait();
            

        }


        bool firstRun = true;
        protected override async void OnAppearing()
        {
            
            if (MyMap.MapType == MapType.Hybrid && Preferences.Get("MAP", "H") != "H")
            {
                MyMap.MapType = MapType.Street;
            }
            else if (MyMap.MapType == MapType.Street && Preferences.Get("MAP", "H") != "S")
            {
                MyMap.MapType = MapType.Hybrid;
            }

            await CheckPermission();

            if(firstRun)
            {
                
                MyMap.IsVisible = false;
                firstRun = false;
                //updateMap();
                //MainThread.BeginInvokeOnMainThread(() =>
                //{
                //    // Code to run on the main thread
                //});

                //_ = Task.Run(async () => { await updateMap(); });

                if(Device.RuntimePlatform == Device.iOS)
                {
                    await updateMap();
                }
                else
                {
                    await Task.Run(() => TryChatConnect());
                    await Task.Run(() => updateMap());
                    
                }
                
            }
        }
        private async Task CheckPermission()
        {
            var LocatorStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            //must be on main thread

            if (LocatorStatus == PermissionStatus.Granted)
            {

                return;
            }
            else
            {
                LocatorStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                Task.Delay(3000).Wait();
                await CheckPermission();
            }
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
                    DisplayAlert("Error", "Realtime service Connection failed", "Cancel");
                }
            }
            else
            {
                _chat.Register(Mycreds);
            }
        }

        static HttpClient client = new HttpClient();

        
        static Uri Uri;
        List<UserProp> UserList = new List<UserProp>();

        public async Task GetUriAsync(bool reload)
        {
            
            Uri = new Uri(await Utility.GetUri(reload));
        }

        
        private async Task updateMap()
        {

            
            //var LocatorStatus =await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
               //must be on main thread

            //if (LocatorStatus != PermissionStatus.Granted)
            //{
            //    LocatorStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            //    Task.Delay(500).Wait();
            //    await updateMap();
            //}
            if(1==2)
            {
                //
            }
            else
            {
                try
                {
                    
                   //check if gps is enabled
                   
                    var posGPS = await Utility.GetPos(false);

                    // get position and update URI 

                    Task.Delay(1000).Wait();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(posGPS.Latitude, posGPS.Longitude), Distance.FromKilometers(0.3)));

                        MyMap.IsVisible = true;

                        MyMap.IsShowingUser = true;
                        
                    });
                    await GetUriAsync(false);

                    //MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(36.731129, 10.2387584), Distance.FromKilometers(0.3)));
                    
                    
                    //TryChatConnect();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        MyMap.CustomPins = new List<CustomPins>();

                    });

                    await UpdateLists();
                   
                }
                catch (Exception ex)
                {
                    await DisplayAlert("error", ex.Message, "OK");
                    await Task.Delay(2000);
                    //await DisplayAlert("error", "balizz open geolocation ", "OK");
                   
                    await updateMap();
                }


            }

        }

         
        private async Task UpdateLists()
        {
            if (Uri != null)
            {
                try
                {
                    UserList.Clear();

                    string res;
                    
                    res = await Utility.GetList(Uri.ToString(), Mycreds); // PERFORMANCE HIT
                   
                    if (res != "ERROR")
                    {
                        UserList = JsonConvert.DeserializeObject<List<UserProp>>(res);

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            MyMap.Pins.Clear();
                            if (Preferences.Get("T", "") == "Client")
                                PopulateCustomTaxi(); //=========================== populate map with pins GENERIC
                            else
                                PopulateCustomClient(); //============== populate map with pins OTHER

                        });
                       
                    }
                    if(res == "ERROR1")
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

                pin.MarkerClicked += pin_clicked;



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


               

                //pin.MarkerClicked += pin_clicked;


                MyMap.CustomPins.Add(pin);
                MyMap.Pins.Add(pin);


            }
        }

        public async Task SendUpdate()
        {
            var posGPS = await Utility.GetPos(true);

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
            DisplayAlert("Sent", "sending shit request", "Kk");
            CustomPins selected_pin = (CustomPins)sender;

            
            int secret = rn.Next(1, 9999);

            _chat.AddPendingReq(secret, selected_pin.UID);

            try
            {
                
                _chat.SendPing(selected_pin.UID, Mycreds,secret);
                
            }
            catch (Exception)
            {
                TryChatConnect();
                DisplayAlert("Error", "connection fail try again", "Ok");

            }

            

            //PhoneDialer.Open(selected_pin.Phone.ToString());
            //Launcher.OpenAsync(new Uri("tel:"+selected_pin.phone.ToString()));
        }

        private void Msgs_taped(object send,EventArgs e)
        {
            DisplayAlert("test", "show msgs or naah xD ?", "Idk");
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
               
                _chat.CreateRoom(Mycreds,RID,uid);
                DisplayAlert("Accepted", "request accepted you can chat now ", "Ok");




                Navigation.PushAsync(new MessagesListPage());

            }
            else
            {
                DisplayAlert("ERROR", "SECRET MISSMATCH", "OK"); //// FOR TEST ONLY /// PROB CHALLENGE??
            }

        }

        private void RoomCreate(string RID)
        {
            // msglist += RID
            // navigation = MsgPage


            
            //DisplayAlert("CREATED", "room: " + RID, "Ok"); // DELETE, THIS FOR TESTING ONLY

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using pikappDes.Utils.modals;
using pikappDes.Utils;

namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackingMap : ContentPage
    {
        IChat _chat;
        Creds myCreds;
        public TrackingMap(Creds MyCreds,UserProp userProp)
        {
            _chat = DependencyService.Get<IChat>();
            InitializeComponent();
            

            _chat.RecUserLoc(RecUserProp);
            myCreds = MyCreds;
            user = userProp;

            MyMap.IsShowingUser = true;

            MyMap.CustomPins = new List<CustomPins>();
            //MyMap.Pins.Clear();
            //MyMap.CustomPins.Clear();
            RefreshLocation();
        }


        public bool refresh = true;
        public bool firstRun = true;
        public UserProp user = new UserProp();
        public async Task RefreshLocation()
        {
            if(!firstRun)
            {
                await Task.Delay(5000);
                
            }
            firstRun = false;
            RequestUserProp();

            

        }

        public void RequestUserProp()
        {
            _chat.GetUserLocation(myCreds, user.UID);
        }

        public void RecUserProp(string usrLocation)
        {
           
            user.pos = usrLocation;
            MyMap.CustomPins.Clear();
            MyMap.Pins.Clear();
            CustomPins pin = new CustomPins
            {
                name = user.name,
                Label = user.name,
                Address = user.pos,

                UID = user.UID,
                Type = PinType.Generic,

                phone = user.phone,
                Position = new Position(Convert.ToDouble(user.pos.Split('/')[0].Replace('.', ',')), Convert.ToDouble(user.pos.Split('/')[1].Replace('.', ',')))

            };

            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(pin.Position.Latitude, pin.Position.Longitude), Distance.FromKilometers(0.3)));


            MyMap.CustomPins.Add(pin);
            MyMap.Pins.Add(pin);

           
            if(refresh)
            {

                RefreshLocation();
            }
        }


        protected override void OnAppearing()
        {
            //CustomPins pin = new CustomPins
            //{
            //    name = user.name,
            //    Label = user.name,
            //    Address = user.pos,

            //    UID = user.UID,
            //    Type = PinType.Generic,

            //    phone = user.phone,
            //    Position = new Position(Convert.ToDouble(user.pos.Split('/')[0]), Convert.ToDouble(user.pos.Split('/')[1]))

            //};
            //MyMap.CustomPins.Clear();
            //MyMap.Pins.Clear();
            //MyMap.CustomPins.Add(pin);
            //MyMap.Pins.Add(pin);
        }

        protected override void OnDisappearing()
        {
            refresh = false;
        }


    }
}
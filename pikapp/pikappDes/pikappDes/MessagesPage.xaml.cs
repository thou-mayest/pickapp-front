using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pikappDes.Utils;
using pikappDes.Utils.modals;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagesPage : ContentPage
    {
        IChat _chat;
        Creds Mycreds = new Creds();

        public string rid;
        public UserProp ThisUser; 
        //public List<MessageModel> Messages = new List<MessageModel>();

        ObservableCollection<MessageModel> messagesCollection = new ObservableCollection<MessageModel>();
        public ObservableCollection<MessageModel> messages_collection { get { return messagesCollection; } }

        public MessagesPage(string RID,UserProp user,MessageModel Lmsg)
        {
            _chat = DependencyService.Get<IChat>();


            InitializeComponent();

            Enum.TryParse(Preferences.Get("T", "Client"), out ClienType type);
            Mycreds.UID = Preferences.Get("UID", "");
            Mycreds.SID = Preferences.Get("SID", "");
            Mycreds.type = type;

            
            _chat.OnError(DisplayError);
            _chat.DirectMsg(DMrec);
            _chat.RecMessages(RecMsgs);

            UsrName.Text = user.name;
            rid = RID;
            //this.Title = user.name; Put in label maybe ? 
            //Messages.Add(Lmsg);
            ThisUser = user;

            MessagesListView.ItemsSource = messagesCollection;
            MessagesListView.ItemSelected += MessagesListView_ItemSelected;
            messagesCollection.Add(Lmsg);
            

        }

        private void MessagesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MessageModel;

            if (item == null)
                return;

            MessagesListView.SelectedItem = null;
        }

        public void DMrec(MessageModel msg , string RID)
        {
            if (rid == RID)
            {
                //Messages.Add(msg);
                messagesCollection.Add(msg);

            }
            

        }

        private void Send_Msg(object sender, EventArgs e)
        {

            if (MsgEntry.Text != "" || MsgEntry.Text != string.Empty)
            {
                _chat.SendMsg(Mycreds, rid, MsgEntry.Text);

                MessageModel mymsg = new MessageModel()
                {
                    Sender = Mycreds.UID,
                    msg = MsgEntry.Text,
                    time = DateTime.Now
                };

                messagesCollection.Add(mymsg);
                MsgEntry.Text = "";
                MessagesListView.ScrollTo(messagesCollection[messagesCollection.Count - 1], ScrollToPosition.End, true);
            }

            //bool connected;
            //try
            //{
            //    connected = _chat.IsConnected();
            //}
            //catch (Exception)
            //{

            //    connected = false;
            //}
            //if (!connected)
            //{
            //    try
            //    {
            //        await _chat.Connect();
            //        await _chat.Register(Mycreds);
            //    }
            //    catch (Exception)
            //    {
            //        await DisplayAlert("Error", "Connection failed", "Cancel");
            //    }


            //}
            //try
            //{
            //    await _chat.SendPing(Msg.Text, Mycreds);
            //}
            //catch (Exception)
            //{

            //    await DisplayAlert("Error", "Connection failed", "Cancel") ;
            //}

        }

        protected override void OnAppearing()
        {

            _chat.GetMessages(Mycreds, rid);
            MessagesListView.ScrollTo(messagesCollection[messagesCollection.Count -1], ScrollToPosition.End, false);

            //DisplayAlert("Msg update", Messages.Count.ToString(), "ok");

            //MessageModel testmodel = new MessageModel()
            //{
            //    Sender = Mycreds.UID,
            //    msg = "my local test message",
            //    time = DateTime.Now
            //};

            //messagesCollection.Insert(0,testmodel);
        }

        public void RecMsgs(List<MessageModel> msgs)
        {
            messagesCollection.Clear();
            foreach(MessageModel msg in msgs)
            {
                messagesCollection.Add(msg);
            }
            MessagesListView.ScrollTo(messagesCollection[messagesCollection.Count - 1], ScrollToPosition.End, false);
        }

        public void DisplayError(string msg)
        {
            DisplayAlert("from serv", "ERROR FRROM SERVER", "Cancle");
        }

        private void Call_Button_Clicked(object sender, EventArgs e)
        {
            //PhoneDialer.Open(selected_pin.Phone.ToString());
            //Launcher.OpenAsync(new Uri("tel:"+selected_pin.phone.ToString()));
            Launcher.OpenAsync(new Uri("tel:" + ThisUser.phone));
        }

        private void ShowOnMap_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TrackingMap(Mycreds,ThisUser));
        }
    }
}
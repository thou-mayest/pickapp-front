using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using pikappDes.Utils;
using System.Collections.ObjectModel;
using pikappDes.Utils.modals;  

namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagesListPage : ContentPage
    {

        IChat _chat;
        Creds MyCreds;

        ObservableCollection<ChatRoomProp> ChatRoomsCollection = new ObservableCollection<ChatRoomProp>();
        public ObservableCollection<ChatRoomProp> Chatrooms { get { return ChatRoomsCollection; } }
        public MessagesListPage()
        {
            
            
            InitializeComponent();

            _chat = DependencyService.Get<IChat>();
            
            _chat.OnError(OnChatError);
            _chat.RecMyrooms(RecMyRooms);


            ChatRoomsListView.ItemsSource = ChatRoomsCollection;
            ChatRoomsListView.ItemSelected += ChatRoomsListView_ItemSelected;


            Enum.TryParse(Preferences.Get("T", "Client"), out ClienType StoredTpe);
            MyCreds = new Creds()
            {
                //    
                //Mycreds.UID = Preferences.Get("UID", "");
                //Mycreds.SID = Preferences.Get("SID", "");
                //Mycreds.type = type;

                UID = Preferences.Get("UID", ""),
                SID = Preferences.Get("SID", ""),
                type = StoredTpe,

            };

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
                    _chat.Register(MyCreds);
                }
                catch (Exception)
                {
                    DisplayAlert("Error", "Realtime service Connection failed", "Cancel");
                }
            }
        }

        private void ChatRoomsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
            var item = e.SelectedItem as ChatRoomProp;
            if (item == null)
                return;
            Navigation.PushAsync(new MessagesPage(item.RID,item.Duser,item.LastMessage));

            ChatRoomsListView.SelectedItem = null;

        }

        public void GetMyRooms()
        {
            if(!_chat.IsConnected())
            {
                //TryChatConnect();

                DisplayAlert("messagelistpage", "waiting for chat hub", "OK");
            }

            _chat.GetMyRooms(MyCreds);
        }


        public void RecMyRooms(List<ChatRoomProp> ResChatRooms)
        {
            try
            {
                ChatRoomsCollection.Clear();
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
                foreach (ChatRoomProp item in ResChatRooms)
                {
                    //var existingItem = ChatRoomsCollection.Where(x => x.RID == item.RID);
                    //if (existingItem == null)
                    //{

                    //}
                    ChatRoomsCollection.Insert(0,item);
                }

                
            }
            catch (Exception x)
            {

                DisplayAlert("error",x.Message,"OK");
            }

            

            //try
            //{
            //    foreach (ChatRoomProp item in ResChatRooms)
            //    {
            //        try
            //        {
            //            var existingItem = ChatRoomsCollection.Where(x => x.RID == item.RID);
            //            if (existingItem == null)
            //            {
            //                ChatRoomsCollection.Insert(0, item);
            //            }
            //        }
            //        catch (Exception)
            //        {
            //            ChatRoomsCollection.Insert(0 ,item);
            //        }
                    


            //        DisplayAlert("item", RresChatRooms.Count.ToString(), "Ok");

            //    }
            //}
            //catch (Exception x)
            //{

            //    DisplayAlert("error",x.Message,"OK");
            //}

            

            //ChatRoomsCollection.Insert(0, new ChatRoomProp { RID = "TEST LOCAL RID", Duser = DuserLocal, });


            // RIDs seperated with  "." 
            //get profile data of for preview  <<=====
            //popluate list with rooms 

        }
        protected override void OnAppearing()
        {
            //TryChatConnect();
            GetMyRooms();
        }

        private void OnChatError(string msg)
        {
            DisplayAlert("Error", msg, "Ok");
        }

        //private void Frame_Focused(object sender, FocusEventArgs e)
        //{
        //    var Selected = e.VisualElement as Frame;

        //    Selected.BorderColor = Color.Red;
        //}
    }
}
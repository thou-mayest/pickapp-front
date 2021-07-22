using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pikappDes.Utils;
using pikappDes.Utils.modals;
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
        public MessagesPage()
        {
            _chat = DependencyService.Get<IChat>();

            InitializeComponent();
            Enum.TryParse(Preferences.Get("T", "Client"), out ClienType type);
            Mycreds.UID = Preferences.Get("UID", "");
            Mycreds.SID = Preferences.Get("SID", "");
            Mycreds.type = type;

           
            _chat.OnError(DisplayError);



        }



        private async void Send_Msg(object sender, EventArgs e)
        {

            if (!_chat.IsConnected())
            {
                try
                {
                    await _chat.Connect();
                    await _chat.Register(Mycreds);
                }
                catch (Exception)
                {
                    await DisplayAlert("Error", "Connection failed", "Cancel");
                }


            }
            try
            {
                await _chat.SendPing(Msg.Text, Mycreds);
            }
            catch (Exception)
            {

                await DisplayAlert("Error", "Connection failed", "Cancel") ;
            }
            
        }

        public void DisplayError()
        {
            DisplayAlert("from serv", "ERROR FRROM SERVER", "Cancle");
        }

        
    }
}
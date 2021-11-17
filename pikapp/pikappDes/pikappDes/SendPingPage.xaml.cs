using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using pikappDes;
using pikappDes.Utils;
using pikappDes.Utils.modals;
using Rg.Plugins.Popup.Extensions;

namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendPingPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        IChat _chat;
        Creds Mycreds;
        string SelectedUID;
        public SendPingPage(string PinUID,Creds creds)
        {
            _chat = DependencyService.Get<IChat>();
            Mycreds = creds;
            SelectedUID = PinUID;

            InitializeComponent();
        }

        readonly Random rn = new Random();
        public void SendReq_Clicked(object sender,EventArgs e)
        {

            AnimatedButton.OnFinishedAnimation += AnimationFinished;
            AnimatedButton.PlayAnimation();

            

            int secret = rn.Next(1, 9999);

            _chat.AddPendingReq(secret, SelectedUID);

            try
            {

                _chat.SendPing(SelectedUID, Mycreds, secret);
                

            }
            catch (Exception)
            {
                //TryChatConnect();
                DisplayAlert("Error", "connection fail try again", "Ok");

            }
        }

        public void AnimationFinished(object sender,EventArgs e)
        {
            Navigation.PopPopupAsync();
        }

        public void Cancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using pikappDes.Utils;

namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessagesListPage : ContentPage
    {

        IChat _chat;
        public MessagesListPage()
        {
            _chat = DependencyService.Get<IChat>();

            _chat.RecMyrooms(RecMyRooms);
            InitializeComponent();
        }


        public void RecMyRooms(string RIDs)
        {
            // RIDs seperated with  "." 
            //get profile data of for preview  <<=====
            //popluate list with rooms 

            DisplayAlert("ROOMS", RIDs, "OK");
        }

    }
}
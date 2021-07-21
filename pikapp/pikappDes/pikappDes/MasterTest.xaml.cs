using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterTest : ContentPage
    {

        ObservableCollection<NewMainMasterMasterMenuItem> MasterMenuItems = new ObservableCollection<NewMainMasterMasterMenuItem>();
        //MasterMenuItems.Add(new NewMainMasterMasterMenuItem{ Id = 1, Title = "TEST title" });
        //MasterMenuItems.Add(new NewMainMasterMasterMenuItem { Id = 2, Title = "testing two" });

        public MasterTest()
        {
            
            InitializeComponent();

            LoadInfo();
        }
        string usrname, number;

        public void LoadInfo()
        {
            // ================================== load menu item

            MenuListViw.ItemsSource = MasterMenuItems;
            MasterMenuItems.Add(new NewMainMasterMasterMenuItem() { Id = 0, Title = "Map", TargetType = typeof(NewMainMasterDetail),ImageSource="RAW_map.png" });
            MasterMenuItems.Add(new NewMainMasterMasterMenuItem() { Id = 1, Title = "Messages" ,TargetType =typeof(MessagesPage), ImageSource="RAW_msg.svg"});
            MasterMenuItems.Add(new NewMainMasterMasterMenuItem() { Id = 2, Title = "Options" , TargetType = typeof(OptionsPage),ImageSource="RAW_options.svg" });
            MasterMenuItems.Add(new NewMainMasterMasterMenuItem() { Id = 3, Title = "History", TargetType = typeof(LoadingPage),ImageSource="RAW_history.svg" });
            MasterMenuItems.Add(new NewMainMasterMasterMenuItem() { Id = 4, Title = "Pro Account", TargetType = typeof(OptionsPage) ,ImageSource="RAW_proAcc.svg"});
            MasterMenuItems.Add(new NewMainMasterMasterMenuItem() { Id = 5, Title = "Disconnect" , ImageSource="RAW_disconnect.svg"});
            // ================================== load NAME & PHONE 

            usrname = Preferences.Get("NAME", "No Val Error");
            number = Preferences.Get("NUMBER", "No Val Error");

            number = Regex.Replace(number, ".{2}","$0 ");
            NameLab.Text = usrname;
            NumberLab.Text = number;

        }
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //NavigationPage main = Application.Current.MainPage;

         
        }
    }
}
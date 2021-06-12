using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace pikappDes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProAccPage : ContentPage
    {
        public ProAccPage()
        {
            InitializeComponent();
        }


        private void LogInButt_Clicked_1(object sender, EventArgs e)
        {

            if (Key_entry.Text == "TAXI")
                Preferences.Set("USR", "T");
            else
                Preferences.Set("USR", "C");
        }
    }
}
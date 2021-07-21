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
    public partial class OptionsPage : ContentPage
    {
        public OptionsPage()
        {
            InitializeComponent();
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if(Check_box.IsChecked)
            {
                Checked();
            }
            else
            {
                NotChecked();
            }
        }

        public void Checked()
        {
            Preferences.Set("MAP", "H");
            
        }
        public void NotChecked()
        {
            Preferences.Set("MAP", "S");
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Check_box.IsChecked = !(Check_box.IsChecked);
        }
    }
}
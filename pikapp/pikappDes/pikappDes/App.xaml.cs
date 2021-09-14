using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using pikappDes.Utils;
namespace pikappDes
{
    public partial class App : Application
    {
        
        public App()
        {


            

            InitializeComponent();

            var ChatService = new ChatHub();
            DependencyService.RegisterSingleton<IChat>(ChatService);

            MainPage = new  NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

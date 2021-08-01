using System;
using System.Collections.Generic;
using System.Linq;
using Rg.Plugins.Popup;
using Foundation;
using UIKit;

namespace pikappDes.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {

            Popup.Init();
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}

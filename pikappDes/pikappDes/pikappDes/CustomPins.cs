using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;
using pikappDes.Utils.modals;

namespace pikappDes
{
    public class CustomPins : Pin
    {
        public string name { get; set; }
        public string phone { get; set; }

        public string pos { get; set; }

        public bool free { get; set; }

        public string UID { get; set; }
        public ClienType type { get; set; }

    }
}

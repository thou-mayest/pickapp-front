using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace pikappDes
{
    public class CustomPins : Pin
    {
        public string Name { get; set; }
        
        public int Phone { get; set; }
        public string Pos { get; set; }
        public bool Free { get; set; }
        public string Pass { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Essentials;
using pikappDes.Utils.modals ;

namespace pikappDes
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MyMsg { get; set; }
        public DataTemplate OtherMsg { get; set; }

        public string UID { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return (((MessageModel)item).Sender == Preferences.Get("UID", "")) ? MyMsg : OtherMsg ;
        }
    }
}

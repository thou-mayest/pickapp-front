using System.Collections.Generic;
using pikappDes.Utils.modals;

namespace pikappDes.Utils.modals
{
    public class ChatRoomProp
    {
        public string RID { get; set; }
        public UserProp Duser { get; set; }
        public MessageModel LastMessage { get; set; }

    }
}
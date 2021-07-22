using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using pikappDes.Utils.modals;

namespace pikappDes.Utils
{
    public interface IChat
    {
        Task Connect();
        void OnError(Action action);
        bool IsConnected();
        Task Register(Creds creds);
        Task SendPing(string uid, Creds creds);
        void OnPing(Action action);
        void OnMsgRec(Action action);
        Task Disconnect();
    }
}

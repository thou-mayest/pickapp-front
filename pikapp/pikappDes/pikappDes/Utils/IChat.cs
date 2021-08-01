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
        void OnError(Action<string> action);
        bool IsConnected();
        Task Register(Creds creds);
        Task SendPing(string uid, Creds creds,int secret);
        void AddPendingReq(int secret, string uid);
        void OnPing(Action<string,int> action);
        Task AcceptPing(int secret,string DestUID,Creds creds);
        void OnAccepted(Action<string,string,int> action);
        bool PendingSecret(string uid, int secret);
        void OnMsgRec(Action action);
        Task Disconnect();
    }
}

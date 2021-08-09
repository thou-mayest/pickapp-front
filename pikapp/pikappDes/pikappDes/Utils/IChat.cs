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
        bool IsPendingSecret(string uid, int secret);
        Task CreateRoom(Creds Mycreds, string RID, string UID);
        void RoomCreated(Action<string> action);
        Task GetMyRooms(Creds MyCreds);
        void RecMyrooms(Action<string> action);
        void OnMsgRec(Action action);
        Task Disconnect();
    }
}

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
        void ReRegister(Action action);
        Task SendPing(string uid, Creds creds,int secret);
        void AddPendingReq(int secret, string uid);
        void OnPing(Action<string,int> action);
        Task AcceptPing(int secret,string DestUID,Creds creds);
        void OnAccepted(Action<string,string,int> action);
        bool IsPendingSecret(string uid, int secret);
        Task CreateRoom(Creds Mycreds, string RID, string UID);
        void RoomCreated(Action<string> action);
        Task GetMyRooms(Creds MyCreds);
        void RecMyrooms(Action<List<ChatRoomProp>> action);
        Task GetMessages(Creds ThisCreds, string RID);
        void RecMessages(Action<List<MessageModel>> action);
        Task SendMsg(Creds MyCreds, string RID, string msg);
        void DirectMsg(Action<MessageModel,string> action);
        Task GetUserLocation(Creds MyCreds, string UID);
        void RecUserLoc(Action<string> action);
        Task Disconnect();
    }
}

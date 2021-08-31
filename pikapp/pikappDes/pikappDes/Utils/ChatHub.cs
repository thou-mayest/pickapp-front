using System;
using System.Collections.Generic;
using System.Text;
using pikappDes.Utils;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using pikappDes.Utils.modals;

namespace pikappDes.Utils
{
    public class ChatHub : IChat
    {
        private HubConnection ConnectionHub;

        public string uri;

        public ChatHub()
        {
            BuildConnection();
        }

        public bool IsConnected()
        {
            bool hubDisconnected;
            try
            {
                hubDisconnected = ConnectionHub.State == HubConnectionState.Disconnected ? true : false;
            }
            catch (Exception)
            {
                hubDisconnected = true;
            }
            if (hubDisconnected)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async void BuildConnection()
        {
            //uri = await Utility.GetUri(false);
            ConnectionHub = new HubConnectionBuilder()
                .WithUrl(await Utility.GetUri(true) + "/ChatHub")
                .Build();
        }

        public void OnMsgRec(Action action)
        {
            ConnectionHub.On("Msg", action);
        }
        public void OnPing(Action<string,int> action)
        {
            ConnectionHub.On<string,int>("OnPing", action);
        }
       
        public void OnError(Action<string> action)
        {
            ConnectionHub.On<string>("OnError", action);
        }


        public async Task Connect()
        {
            await ConnectionHub.StartAsync();
        }

       

        public async Task SendPing(string DestUID,Creds creds,int secret)
        {
            await ConnectionHub.InvokeAsync("SendPing", DestUID, creds,secret);
        }

        public async Task AcceptPing(int secret,string DestUID,Creds creds)
        {
            await ConnectionHub.InvokeAsync("AcceptPing", secret,DestUID,creds);
        }

        public void OnAccepted(Action<string,string,int> action)
        {
            ConnectionHub.On<string,string,int>("OnAccepted", action);
            //OnAccepted(string RID,int secret);
        }

        public async Task Register(Creds creds)
        {
            await ConnectionHub.InvokeAsync("RegisterSgID", creds);
        }

        public async Task Disconnect()
        {
            await ConnectionHub.StopAsync();
        }

        Dictionary<string, int> pending = new Dictionary<string, int>();
        public void AddPendingReq(int secret , string uid)
        {

            bool ispending = pending.TryGetValue(uid, out int StoredSecret);

            if (!ispending)
            {
                pending.Add(uid, secret);
            }
            else
            {
                pending[uid] = secret;
            }

            
        }

        public bool IsPendingSecret(string uid, int secret)
        {
            //true if pending UID = giving secret

            pending.TryGetValue(uid, out int StoredSecret);

            return (StoredSecret == secret) ? true : false;
        }

        public async Task CreateRoom(Creds Mycreds,string RID,string UID)
        {
            await ConnectionHub.InvokeAsync("CreateRoom",Mycreds,RID,UID);
        }
        public void RoomCreated(Action<string> action)
        {
            ConnectionHub.On<string>("RoomCreated",action);
        }

        public async Task GetMyRooms(Creds MyCreds)
        {
            await ConnectionHub.InvokeAsync("GetMyRooms", MyCreds);
        }

        public void RecMyrooms(Action<List<ChatRoomProp>> action)
        {
            ConnectionHub.On<List<ChatRoomProp>>("RecMyrooms", action);
        }

        public async Task GetMessages(Creds ThisCreds, string RID)
        {
            await ConnectionHub.InvokeAsync("GetMessages", ThisCreds, RID);
        }

        public void RecMessages(Action<List<MessageModel>> action)
        {
            ConnectionHub.On<List<MessageModel>>("RecRoomMsgs",action);
        }

        public async Task SendMsg(Creds MyCreds,string RID,string msg)
        {
            await ConnectionHub.InvokeAsync("SendMsg", MyCreds, RID, msg);
        }

        public void DirectMsg(Action<MessageModel,string> action)
        {
            ConnectionHub.On<MessageModel,string>("DirectMsg", action);
        }
        public async Task GetUserLocation(Creds MyCreds,string UID)
        {
            await ConnectionHub.InvokeAsync("GetUserLocation", MyCreds,UID);
        }
        public void RecUserLoc(Action<string> action)
        {
            ConnectionHub.On<string>("RecUserLoc", action);
        }
    }
}

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
        public void OnMsgRec(Action action)
        {
            ConnectionHub.On("Msg", action);
        }
        public void OnPing(Action action)
        {
            ConnectionHub.On("Ping", action);
        }
        public async void BuildConnection()
        {
            uri = await Utility.GetUri();
            ConnectionHub = new HubConnectionBuilder()
                .WithUrl(uri + "/ChatHub")
                .Build();
        }

        public void OnError(Action action)
        {
            ConnectionHub.On("OnError", action);
        }


        public async Task Connect()
        {
            await ConnectionHub.StartAsync();
        }

        public bool IsConnected()
        {
            if(ConnectionHub.State == HubConnectionState.Disconnected)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task SendPing(string DestUID,Creds creds)
        {
            await ConnectionHub.InvokeAsync("SendPing", DestUID, creds);
        }



        public async Task Register(Creds creds)
        {
            await ConnectionHub.InvokeAsync("RegisterSgID", creds);
        }

        public async Task Disconnect()
        {
            await ConnectionHub.StopAsync();
        }
    }
}

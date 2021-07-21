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
        private readonly HubConnection ConnectionHub;


        public ChatHub()
        {
            ConnectionHub = new HubConnectionBuilder()
                .WithUrl("https://669292a580d4.ngrok.io" + "/ChatHub")
                .Build();
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

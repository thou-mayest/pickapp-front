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
        bool IsConnected();
        Task Register(Creds creds);
        Task Disconnect();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    interface IDirectoryHandlerNotifier
    {
        void SendCommand(int id, string[] args, string path);
    }
}

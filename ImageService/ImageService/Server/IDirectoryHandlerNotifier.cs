using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    /// <summary>
    /// allows sending a command to all directory handlers via event
    /// </summary>
    interface IDirectoryHandlerNotifier
    {
        void SendCommand(int id, string[] args, string path);
    }
}

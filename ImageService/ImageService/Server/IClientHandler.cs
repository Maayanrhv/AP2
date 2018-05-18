using ImageService.Controller.Handlers;
using ImageService.Modal;
using System.Net.Sockets;
using System.Threading;

namespace ImageService.Server
{
    //public delegate void TcpClientDelegate(TcpClient client);

    internal interface IClientHandler
    {
        //event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        //event TcpClientDelegate ExcludeClient;

        void HandleClient(TcpClient client);

        void DirectoryHandlerIsBeingClosed(TcpClient client, DirectoryCloseEventArgs e);

        void CloseAllClients();
    }
}
using ImageService.Communication;
using System.Net.Sockets;

namespace ImageService.Server
{
    public interface IClientInformer : IClientHandler
    {
        /// <summary>
        /// send the given client the given message
        /// </summary>
        /// <param name="client">a client</param>
        /// <param name="msg">a message to send to the client</param>
        void InformClient(TcpClient client, CommunicationProtocol msg);
    }
}
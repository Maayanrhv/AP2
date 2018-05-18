﻿using ImageService.Communication;
using ImageService.Controller.Handlers;
using ImageService.Modal;
using System.Net.Sockets;
using System.Threading;

namespace ImageService.Server
{
    public delegate void clientDelegation(TcpClient client);

    internal interface IClientHandler
    {
        event clientDelegation CloseClientEvent;
        
        void HandleClient(TcpClient client);

        void InformClient(TcpClient client, CommunicationProtocol msg);

        void CloseAllClients();
    }
}
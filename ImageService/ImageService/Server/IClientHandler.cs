﻿using System;
using System.Net.Sockets;
using ImageService.Modal.Event;

namespace ImageService.Server
{
    /// <summary>
    /// handling a client on tcp connection. can handle requests, inform the client, 
    /// stop connection, and inform about a client that is being closed.
    /// </summary>
    public interface IClientHandler
    {
        /// <summary>
        /// inform about a client that closes the connection.
        /// </summary>
        event EventHandler<ClientEventArgs> ClientClosedConnectionEvent;
        
        /// <summary>
        /// answering client's requests and sending the client info.
        /// </summary>
        /// <param name="client">the client</param>
        void HandleClient(TcpClient client);
        
        /// <summary>
        /// stop handling all clients that are connected.
        /// </summary>
        void StopHandlingClients();
    }
}
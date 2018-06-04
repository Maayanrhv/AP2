using System;
using System.Net.Sockets;

namespace ImageService.Modal.Event
{
    /// <summary>
    /// carring information about a client
    /// </summary>
    public class ClientEventArgs : EventArgs
    {
        /// <summary>
        /// the client tcp
        /// </summary>
        #region client property
        public TcpClient Client
        {
            set;
            get;
        }
        #endregion
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="client">a client</param>
        public ClientEventArgs(TcpClient client)
        {
            this.Client = client;
        }
    }
}

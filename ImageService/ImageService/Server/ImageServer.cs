using System;
using ImageService.Communication;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageService.Server
{
    /// <summary>
    /// ImageServer creats removed_Handlers to every directory being watched, and
    /// sends commands to those removed_Handlers via event.
    /// </summary>
    class ImageServer : IDirectoryHandlerNotifier
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private TcpListener listener;
        private IClientHandler ch;
        private List<TcpClient> allClients;
        private bool serverIsOn;
        //private static Mutex mutex = new Mutex();
        #endregion

        #region Properties
        // The event that notifies the Directoryhandlers about a new Command being recieved
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        #endregion

        /// <summary>
        ///  constructor.
        /// </summary>
        /// <param name="controller">commands controller object.</param>
        /// <param name="logging">logger to pass messages to the log.</param>
        public ImageServer(IImageController controller, ILoggingService logging)
        {
            m_controller = controller;
            m_logging = logging;
            ch = new ClientHandler(m_controller, m_logging, this);
            CreateDirectoryHandlers();
            this.allClients = new List<TcpClient>();
            serverIsOn = true;
        }



        /// <summary>
        /// reads from App.config the path removed_Handlers that are specified; to which 
        /// directories the service will listen.
        /// </summary>
        public void CreateDirectoryHandlers()
        {
            string allDirectories = ConfigurationManager.AppSettings["Handler"];
            string[] paths = allDirectories.Split(';');
            foreach (string path in paths) { ListenToDirectory(path); }
        }

        /// <summary>
        /// sets a handler to the directory given in path parameter. 
        /// the event handler of the handler registers to the server's commands event.
        /// </summary>
        /// <param name="path">path to a directory</param>
        public void ListenToDirectory(string path)
        {
            IDirectoryHandler handler = new DirectoyHandler(m_controller, m_logging);
            this.CommandRecieved += handler.OnCommandRecieved;
            handler.DirectoryClose += HandlerIsBeingClosed;
            handler.StartHandleDirectory(path);
        }

         /// <summary>
         /// the function sends to all the removed_Handlers a closeCommand and
         /// takes them off the command event. called when the service is closing.
         /// </summary>
        public void ServiceIsclosing()
        {
            ch.CloseAllClients();
            CloseAllDirHandlers();
            serverIsOn = false;
            //CloseAllClientsConn();
        }

        private void CloseAllDirHandlers()
        {
            foreach (EventHandler<CommandRecievedEventArgs> handler in CommandRecieved.GetInvocationList())
            {
                handler(this, new CommandRecievedEventArgs((int)CommandEnum.CloseAllCommand, null, null));
                CommandRecieved -= handler;
            }
        }
        //private void CloseAllClientsConn()
        //{
        //    foreach (TcpClient client in this.allClients)
        //    {
        //        client.Close();
        //        serverIsOn = false;
        //    }
                
        //}

        /// <summary>
        /// this is an event handler: when a handler is being closed this function
        /// will be called and remove HandlerIsBeingClosed from the handler's event, 
        /// and remove OnCommandRecieved(handler's function) from CommandRecieved 
        /// event.
        /// </summary>
        /// <param name="sender">who called the func HandlerIsBeingClosed </param>
        /// <param name="e">arguments</param>
        public void HandlerIsBeingClosed(object sender, DirectoryCloseEventArgs e) //HANDLER TELLING ME IT IS BEING CLOSED - TELL  CLIENT HANDLER
        {
            if (sender is IDirectoryHandler)
            {
                // inform all clients handler is being closed
                foreach (TcpClient client in this.allClients)
                    this.ch.DirectoryHandlerIsBeingClosed(client, e);
                ((IDirectoryHandler)sender).DirectoryClose -= HandlerIsBeingClosed;
                this.CommandRecieved -= ((IDirectoryHandler)sender).OnCommandRecieved;
            }
        }

        /// <summary>
        ///  sends the command given in the parameters to all the functions in the event
        ///  CommandRecieved.
        /// </summary>
        /// <param name="id">command id - from CommandEnum</param>
        /// <param name="args">arguments needed for the command:
        /// args[0] = the file's name</param>
        /// <param name="path">path to directory, without the file's name</param>
        public void SendCommand(int id, string[] args, string path)
        {
            this.CommandRecieved?.Invoke(this, new CommandRecievedEventArgs(id, args, path));
        }

        
        public void Start()
        {
            string ip = ConfigurationManager.AppSettings["IP"];
            int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            listener = new TcpListener(ep);
            listener.Start();
            m_logging.Log(Messages.ServerWaitsForConnections(), MessageTypeEnum.INFO);

            Task task = new Task(() => {
                while (this.serverIsOn)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        this.allClients.Add(client);
                        m_logging.Log(Messages.ServerGotNewClientConnection(), MessageTypeEnum.INFO);
                        ch.HandleClient(client);
                    }
                    catch (SocketException)
                    {
                        m_logging.Log(Messages.ServerCouldntAcceptClient(), MessageTypeEnum.FAIL);
                        break;
                    }
                }
                m_logging.Log(Messages.ServerStopped(), MessageTypeEnum.INFO);
            });
            task.Start();
        }

        public void Stop()
        {
            listener.Stop();
            m_logging.Log(Messages.ServerClosedConnections(), MessageTypeEnum.INFO);
        }
    }
}
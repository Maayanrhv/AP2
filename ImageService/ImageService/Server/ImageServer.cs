using System;
using ImageService.Communication;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using ImageService.Modal.Event;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using System.Collections.Generic;
using ImageService.Server.ImagesHandling;

namespace ImageService.Server
{
    /// <summary>
    /// ImageServer creats Directory Handler to every directory being watched, and
    /// sends commands to those Handlers via event.
    /// ImageServer is a connective Server that accepts clients and sends them data about 
    /// the Handlers and the Service basic info.
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

        private ImagesPort imagesPort;
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
            ch.ClientClosedConnectionEvent += delegate (Object sender, ClientEventArgs e)
            {
                CloseAndRemoveClient(e.Client);
                m_logging.Log(Messages.ClientClosedConnection(), MessageTypeEnum.WARNING);
            };
            CreateDirectoryHandlers();
            allClients = new List<TcpClient>();
            serverIsOn = true;
            // TODO: interface
            imagesPort = new ImagesPort(m_logging);
        }

        /// <summary>
        /// reads from App.config the path RemovedHandlers that are specified; to which 
        /// directories the service will listen.
        /// </summary>
        public void CreateDirectoryHandlers()
        {
            try
            {
                string allDirectories = ConfigurationManager.AppSettings["Handler"];
                if (allDirectories.Length != 0)
                {
                    string[] paths = allDirectories.Split(';');
                    foreach (string path in paths) { ListenToDirectory(path); }
                }
            } catch(Exception)
            {
                this.m_logging.Log("no directory handlers", MessageTypeEnum.WARNING);
            }
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
        /// the function sends to all the RemovedHandlers a closeCommand and
        /// takes them off the command event. called when the service is closing.
        /// </summary>
        public void ServiceIsclosing()
        {
            this.Stop();
            CloseAllDirHandlers();
        }

        /// <summary>
        /// closing all directory handlers
        /// </summary>
        private void CloseAllDirHandlers()
        {
            try
            {
                foreach (EventHandler<CommandRecievedEventArgs> handler in CommandRecieved.GetInvocationList())
                {
                    handler(this, new CommandRecievedEventArgs((int)CommandEnum.CloseAllCommand, null, null));
                    CommandRecieved -= handler;
                }
            } catch (Exception)
            {

            }
        }

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
                string[] path = { e.DirectoryPath };
                CommunicationProtocol msg = new CommunicationProtocol((int)CommandEnum.CloseHandlerCommand, path);
                InformClients(msg);
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

       /// <summary>
       /// closing connection with the Client and removing him from clients list.
       /// </summary>
       /// <param name="cl">a Client</param>
        private void CloseAndRemoveClient(TcpClient cl)
        {
            try
            {
                allClients.Remove(cl);
                cl.Close();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// sends a message to all clients in clients list. if sending a message to one of the clients fails-
        /// the connection with the Client closes and the Client is removed from clients list.
        /// </summary>
        /// <param name="msg">a message to sent to all clients</param>
        private void InformClients(CommunicationProtocol msg)
        {
            List<TcpClient> clients = new List<TcpClient>(this.allClients);
            foreach (TcpClient client in clients)
            {
                try
                {
                    ch.InformClient(client, msg);
                }
                catch (Exception ex)
                {
                    CloseAndRemoveClient(client);
                    m_logging.Log(Messages.CanNotCommunicate_ClientRemoved(ex), MessageTypeEnum.FAIL);
                }
            }
        }

        /// <summary>
        /// opens communication protocol- server starts listening to clients.
        /// </summary>
        public void Start()
        {
            m_logging.AddEvent(delegate (Object sender, MessageRecievedEventArgs e)
            {
                string[] logs = { e.Status.ToString() + " " + e.Message };
                CommunicationProtocol msg = new CommunicationProtocol((int)CommandEnum.GetLogCommand, logs);
                InformClients(msg);
            });

            //string ip = ConfigurationManager.AppSettings["IP"];
            //int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            //IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            listener = new TcpListener(ep);
            listener.Start();
            m_logging.Log(Messages.ServerWaitsForConnections(), MessageTypeEnum.INFO);
            Task task = new Task(() =>
            {
                while (this.serverIsOn)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        ch.HandleClient(client);
                        this.allClients.Add(client);
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
            this.imagesPort.Start();
        }

    /// <summary>
    /// closes communication protocol- server stops listening to clients.
    /// all existing Client connections are being closed.
    /// </summary>
    public void Stop()
        {
            serverIsOn = false;
            this.imagesPort.Stop();
            ch.StopHandlingClients();
            listener.Stop();
            m_logging.Log(Messages.ServerClosedConnections(), MessageTypeEnum.INFO);
        }
    }
}
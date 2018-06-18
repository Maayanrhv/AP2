using ImageService.Infrastructure;
using ImageService.Logging;
using ImageService.Modal.Event;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ImageService.Server.ImagesHandling
{
    /// <summary>
    /// shows messages that were sent from the program on the service log.
    /// contains text to inform the log's related to the Android connection.
    /// </summary>
    public class ImagesPortLogger : ILoggingService
    {
        private ILoggingService m_logging;
        private string IMAGES_PORT = "::ImagesPort:: ";

        public ImagesPortLogger(ILoggingService logger) { m_logging = logger; }

        /// <summary>
        /// adds a ILoggingService event handler.
        /// </summary>
        /// <param name="func">event handler: function to be called when the event
        /// is invoked</param>
        public void AddEvent(EventHandler<MessageRecievedEventArgs> func)
        {
            m_logging.AddEvent(func);
        }

        /// <summary>
        /// shows a message on the log
        /// </summary>
        /// <param name="message">message content</param>
        /// <param name="type">type of the message</param>
        public void Log(string message, MessageTypeEnum type)
        {
            m_logging.Log(IMAGES_PORT+message, type);
        }
    }

    /// <summary>
    /// ImagePort connects to an Android device, recieves images from it
    /// and puts them in a handler directory given from the App.config.
    /// </summary>
    public class ImagesPort : IPort
    {
        private string IP_ADDRESS = "127.0.0.1";
        private int PORT = 8500;

        private ILoggingService m_logging;
        private TcpListener listener;
        private bool serverIsOn;
        private List<TcpClient> allImageProviders;
        private IClientHandler iph;

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="logger">logger to pass messages to the log.</param>
        public ImagesPort(ILoggingService logger) {
            m_logging = new ImagesPortLogger(logger);
            allImageProviders = new List<TcpClient>();
            iph = new ImageProviderHandler(m_logging);
            iph.ClientClosedConnectionEvent += delegate (Object sender, ClientEventArgs e)
            {
                CloseAndRemoveClient(e.Client);
                m_logging.Log(Messages.ClientClosedConnection(), MessageTypeEnum.WARNING);
            };
            serverIsOn = true;
        }

        /// <summary>
        /// closes communication protocol with Android devices.
        /// </summary>
        public void Stop() {
            serverIsOn = false;
            iph.StopHandlingClients();
            listener.Stop();
            m_logging.Log(Messages.ServerClosedConnections(), MessageTypeEnum.INFO);
        }

        /// <summary>
        /// opens communication protocol with Android devices.
        /// </summary>
        public void Start()
        {
            //IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IP_ADDRESS), PORT);
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, PORT);
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
                        iph.HandleClient(client);
                        this.allImageProviders.Add(client);
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

        /// <summary>
        /// closing connection with the Client and removing it from clients list.
        /// </summary>
        /// <param name="cl">a Client</param>
        private void CloseAndRemoveClient(TcpClient cl)
        {
            try
            {
                allImageProviders.Remove(cl);
                cl.Close();
            }
            catch (Exception) { }
        }
    }
}

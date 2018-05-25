using ImageService.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using ImageService.Logging;
using ImageService.Infrastructure.Enums;
using GUI.Communication;
using System.Configuration;
using ImageService;

namespace ImageServiceGUI.Communication
{
    /// <summary>
    /// responsible for the connection with Server.
    /// this is a singleton class.
    /// </summary>
    public sealed class SingletonClient
    {
        #region Members
        private static SingletonClient instance = null;
        TcpClient client;
        private static bool stop = false;
        private static Mutex mutex = new Mutex();
        #endregion

        #region Notify Changed
        /// <summary>
        /// notify about message from Server
        /// </summary>
        public event EventHandler<ServiceInfoEventArgs> MsgRecievedFromServer;
        /// <summary>
        /// notify connection is lost
        /// </summary>
        public event EventHandler<ConnectionArgs> ConnectionIsBroken;
        #endregion

        /// <summary>
        /// returns the single instance of SingletonClient class.
        /// </summary>
        public static SingletonClient getInstance
        {
            get
            {
                if (instance == null)
                    instance = new SingletonClient();
                return instance;
            }
        }

        /// <summary>
        /// send to Server a command to close the given directory handler
        /// </summary>
        /// <param name="handlers">a directory handler to close</param>
        public void CloseHandler(List<string> handlers)
        {
            CommunicationProtocol msg = new CommunicationProtocol(
                (int)CommandEnum.CloseHandlerCommand, handlers.ToArray());
            SendDataToServer(msg);
        }

        /// <summary>
        /// connect to Server. takes the IP and Port from App.config
        /// </summary>
        /// <returns>true if succeeded in connecting to server, false o.w</returns>
        public bool ConnectToServer()
        {
            string ip = ConfigurationManager.AppSettings["IP"];
            int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            this.client = new TcpClient();
            try
            {
                client.Connect(ep);
                RecieveDataFromServer();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// send a given message to Server.
        /// if sending didn't succeed - the connection is closed.
        /// </summary>
        /// <param name="msg">message to send to Server</param>
        public void SendDataToServer(CommunicationProtocol msg)
        {
            try
            {
                string jsonCommand = JsonConvert.SerializeObject(msg);
                NetworkStream stream = client.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);

                mutex.WaitOne();
                writer.Write(jsonCommand);
                mutex.ReleaseMutex();
            }
            catch (Exception)
            {
                ConnectionIsBroken(this, null);
                CloseClient();
            }
        }

        /// <summary>
        /// listens to Server
        /// </summary>
        public void RecieveDataFromServer()
        {
            new Task(() =>
            {
                NetworkStream stream = this.client.GetStream();
                BinaryReader reader = new BinaryReader(stream);
                while (!stop)
                {
                    try
                    {
                        string response = reader.ReadString(); // Wait for response from serve
                        CommunicationProtocol msg = JsonConvert.DeserializeObject<CommunicationProtocol>(response);
                        MsgRecievedFromServer(this, ClientServerArgsParser.Parse(msg));

                        Thread.Sleep(1000); // Update information every 1 second
                    }
                    catch (Exception)
                    {
                        ConnectionIsBroken(this, null);
                        CloseClient();
                    }
                }
                CloseClient();
            }).Start();
        }

        /// <summary>
        /// close the connection with the Server.
        /// </summary>
        public void CloseClient()
        {
            client.Close();
            stop = true;
        }

        /// <summary>
        /// informing the Server the GUI window is being closed, and closing
        /// the connection.
        /// </summary>
        public void StartClosingWindow()
        {
            CommunicationProtocol closeWindow = new CommunicationProtocol((int)CommandEnum.CloseGUICommand, null);
            SendDataToServer(closeWindow);
            CloseClient();
        }
    }
}
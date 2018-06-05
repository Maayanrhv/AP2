using ImageService.Communication;
using ImageService.Infrastructure.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

//TODO: make it not singleton and move it to communication

namespace ImageServiceWeb.Models
{
    /// <summary>
    /// argument for ConnectionIsBroken event
    /// </summary>
    public class ConnectionArgs : EventArgs
    {
        public ConnectionArgs() { }
    }


    // SYNCRONIC CLIENT
    // TODO: close connection properly. close client

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
        public ServiceInfoEventArgs CloseHandler(List<string> handlers)
        {
            CommunicationProtocol msg = new CommunicationProtocol(
                (int)CommandEnum.CloseHandlerCommand, handlers.ToArray());

            ServiceInfoEventArgs ret = SendDataToServer(msg);
            // wait for deletion confirmation
            while (ret.RemovedHandlers == null)
            {
                ret = GetAnswer(true);
            }
            return ret;
        }

        /// <summary>
        /// waits for message from server
        /// added for synchronic comunication.
        /// the function also updates everyone via event about the information from server.
        /// </summary>
        /// <returns>the message from server</returns>
        private ServiceInfoEventArgs GetAnswer(bool informAll)
        {
            NetworkStream stream = this.client.GetStream();
            BinaryReader reader = new BinaryReader(stream);
            string response = reader.ReadString(); // Wait for response from serve
            CommunicationProtocol msg = JsonConvert.DeserializeObject<CommunicationProtocol>(response);
            ServiceInfoEventArgs answer = ClientServerArgsParser.Parse(msg);
            if (informAll)
                MsgRecievedFromServer(this, answer);
            return answer;
        }

        /// <summary>
        /// connect to Server. takes the IP and Port from App.config
        /// </summary>
        /// <returns>true if succeeded in connecting to server, false o.w</returns>
        public ServiceInfoEventArgs ConnectToServer(out bool result)
        {
            string ip = ConfigurationManager.AppSettings["IP"];
            int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            //IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            this.client = new TcpClient();
            try
            {
                client.Connect(ep);
                // get initial information from server
                ServiceInfoEventArgs info = new ServiceInfoEventArgs();
                bool config = false, logs = false;
                ServiceInfoEventArgs answer = GetAnswer(false);
                while (!config || !logs)
                {
                    if (answer.ConfigMap != null) {
                        info.ConfigMap = answer.ConfigMap;
                        config = true;
                    }
                    if (answer.LogsList != null) {
                        info.LogsList= answer.LogsList;
                        logs = true;
                    }
                    answer = GetAnswer(true);
                }
                result = true;
                return info;
            }
            catch (Exception)
            {
                result = false;
                return null;
            }
        }

        /// <summary>
        /// send a given message to Server.
        /// if sending didn't succeed - the connection is closed.
        /// </summary>
        /// <param name="msg">message to send to Server</param>
        public ServiceInfoEventArgs SendDataToServer(CommunicationProtocol msg)
        {
            try
            {
                string jsonCommand = JsonConvert.SerializeObject(msg);
                NetworkStream stream = client.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                mutex.WaitOne();
                writer.Write(jsonCommand);
                mutex.ReleaseMutex();
                // waits for server's answer - synchronic communication.
                return GetAnswer(true);
            }
            catch (Exception)
            {
                ConnectionIsBroken(this, null);
                CloseClient();
                return null;
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

                        //Thread.Sleep(1000); // Update information every 1 second
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
        //public void StartClosingWindow()
        //{
        //    CommunicationProtocol closeWindow = new CommunicationProtocol((int)CommandEnum.CloseGUICommand, null);
        //    SendDataToServer(closeWindow);
        //    CloseClient();
        //}
    }
}
using ImageService.Infrastructure;
using ImageService.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using ImageService.Communication;
using ImageService.Infrastructure.Enums;
using System.Threading;
using ImageService.Controller;
using ImageService.Modal.Event;

namespace ImageService.Server
{
    /// <summary>
    /// handles the server's clients
    /// </summary>
    class ClientHandler : IClientHandler
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private IDirectoryHandlerNotifier m_handlersNotifier;
        private static Mutex Mutex = new Mutex();
        private static bool serverIsOn;
        #endregion

        #region Events
        /// <summary>
        /// informs about a client that closed the connection
        /// </summary>
        public event EventHandler<ClientEventArgs> ClientClosedConnectionEvent;
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller">commands executer</param>
        /// <param name="logging">Shows messages in EventLog</param>
        /// <param name="handlersNotifier">can send a command to all directory handlers via event</param>
        public ClientHandler(IImageController controller, ILoggingService logging,
            IDirectoryHandlerNotifier handlersNotifier)
        {
            m_controller = controller;
            m_logging = logging;
            m_handlersNotifier = handlersNotifier;
            serverIsOn = true;
        }

        /// <summary>
        /// send message to client
        /// </summary>
        /// <param name="client">a connected client</param>
        /// <param name="msg">message to send the client</param>
        public void InformClient(TcpClient client, CommunicationProtocol msg)
        {
            SendDataToClient(msg, client);
        }

        /// <summary>
        /// stop handling all clients. the clients are being close as a result
        /// </summary>
        public void StopHandlingClients()
        {
            serverIsOn = false;
        }

        /// <summary>
        /// send message to client
        /// </summary>
        /// <param name="msg">message to send the client</param>
        /// <param name="client">a connected client</param>
        private void SendDataToClient(CommunicationProtocol msg, TcpClient client)
        {
            string jsonCommand = JsonConvert.SerializeObject(msg);
            SendDataToClient(jsonCommand, client);
        }

        /// <summary>
        /// send message to client
        /// </summary>
        /// <param name="msg">message to send the client</param>
        /// <param name="client">a connected client</param>
        private void SendDataToClient(string msg, TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            SendDataToClient(msg, writer);
        }

        /// <summary>
        /// send message to client
        /// </summary>
        /// <param name="msg">message to send the client</param>
        /// <param name="writer">a writer that writes to a client</param>
        /// <exception>can't send data to Client.</exception>
        private void SendDataToClient(string msg, BinaryWriter writer)
        {
            Mutex.WaitOne();
            writer.Write(msg);
            Mutex.ReleaseMutex();
        }

        /// <summary>
        /// sends App.config content and all existing logs to the client
        /// </summary>
        /// <param name="writer">a writer that writes to a client</param>
        private void SendInitialInfo(BinaryWriter writer)
        {
            bool res1, res2;
            string configs = m_controller.ExecuteCommand(
                (int)CommandEnum.GetConfigCommand, null, out res1);
            string logs = m_controller.ExecuteCommand(
                (int)CommandEnum.GetLogCommand, null, out res2);
            if (res1 && res2)
            {
                SendDataToClient(configs, writer);
                SendDataToClient(logs, writer);
            }
        }

        /// <summary>
        /// handling the client's request
        /// </summary>
        /// <param name="msg">the client's request</param>
        private void HandleRequest(CommunicationProtocol msg)
        {
            CommandEnum id = (CommandEnum)msg.Command_Id;
            bool result;
            if (id == CommandEnum.CloseHandlerCommand)
            {
                foreach (string handlersPath in msg.Command_Args)
                {
                    this.m_handlersNotifier.SendCommand((int)id, null, handlersPath);
                }
            }
            string commandRes = m_controller.ExecuteCommand(msg.Command_Id, msg.Command_Args, out result);
            if (result)
            {
                m_logging.Log(Messages.CommandRanSuccessfully(id), MessageTypeEnum.INFO);
            }
            else
            {
                m_logging.Log(Messages.FailedExecutingCommand(id), MessageTypeEnum.FAIL);
            }
        }

        /// <summary>
        /// listening to the client and answering it's requests.
        /// stops listening(stops the connection) if: Server shut down, client
        /// had disconnected, or if something went wrong and exception was thrown.
        /// </summary>
        /// <param name="client">a client to listen to</param>
        public void HandleClient(TcpClient client)
        {
            new Task(() =>
            {
                NetworkStream stream = client.GetStream();
                BinaryReader reader = new BinaryReader(stream);
                BinaryWriter writer = new BinaryWriter(stream);
                try
                {
                    SendInitialInfo(writer);
                    m_logging.Log(Messages.ServerGotNewClientConnection(), MessageTypeEnum.INFO);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    m_logging.Log(Messages.ErrorSendingConfigAndLogDataToClient(e), MessageTypeEnum.FAIL);
                    return;
                }
                {
                    while (serverIsOn)
                    {
                        try
                        {
                            string requset = reader.ReadString(); // Wait for Client to send request
                            CommunicationProtocol msg = JsonConvert.DeserializeObject<CommunicationProtocol>(requset);
                            if (msg != null)
                            {
                                if (msg.Command_Id == (int)CommandEnum.CloseGUICommand)
                                {
                                    ClientClosedConnectionEvent(this, new ClientEventArgs(client));
                                    break;
                                }
                                else
                                    HandleRequest(msg);
                            }
                            else
                            {
                                m_logging.Log(Messages.ErrorRecievingMessageFromClient(), MessageTypeEnum.FAIL);
                            }
                        }
                        catch (Exception)
                        {
                            m_logging.Log(Messages.ErrorHandlingClient(), MessageTypeEnum.FAIL);
                            break;
                        }
                    }
                    client.Close();
                }
            }).Start();
        }
    }
}

using ImageService.Infrastructure;
using ImageService.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ImageService.Communication;
using ImageService.Infrastructure.Enums;
using System.Threading;
using ImageService.Controller;
using ImageService.Modal;
using ImageService.Controller.Handlers;

namespace ImageService.Server
{
    class ClientHandler : IClientHandler
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private IDirectoryHandlerNotifier m_handlersNotifier;
        //public Mutex Mutex { get; set; }
        private static Mutex Mutex = new Mutex();
        private static bool serverIsOn;
        #endregion

        public ClientHandler(IImageController controller, ILoggingService logging,
            IDirectoryHandlerNotifier handlersNotifier)
        {
            m_controller = controller;
            m_logging = logging;
            m_handlersNotifier = handlersNotifier;
            serverIsOn = true;
        }

        public void InformClient(TcpClient client, CommunicationProtocol msg)
        {
            SendDataToClient(msg, client);
        }

        public void CloseAllClients()
        {
            serverIsOn = false;
        }

        /// <exception>can't send data to client.</exception>
        private void SendDataToClient(CommunicationProtocol msg, TcpClient client)
        {
            string jsonCommand = JsonConvert.SerializeObject(msg);
            NetworkStream stream = client.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Mutex.WaitOne();
            writer.Write(jsonCommand);
            Mutex.ReleaseMutex();
        }
        /// <exception>can't send data to client.</exception>
        private void SendDataToClient(string msg, TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);
            Mutex.WaitOne();
            writer.Write(msg);
            Mutex.ReleaseMutex();
        }
        /// <exception>can't send data to client.</exception>
        private void SendDataToClient(string msg, BinaryWriter writer)
        {
            Mutex.WaitOne();
            writer.Write(msg);
            Mutex.ReleaseMutex();
        }

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

        private void Answer(BinaryWriter writer, CommunicationProtocol msg)
        {
            CommandEnum id = (CommandEnum)msg.Command_Id;
            bool result;
            if (id == CommandEnum.CloseHandlerCommand)
            {
                result = true;
                foreach (string handlersPath in msg.Command_Args)
                {
                    this.m_handlersNotifier.SendCommand((int)id, null, handlersPath);
                }
            }
            else
            {
                string commandRes = m_controller.ExecuteCommand(msg.Command_Id, msg.Command_Args, out result);
                SendDataToClient(commandRes, writer);
            }
            //if (result)
            //{
            //    m_logging.Log(Messages.CommandRanSuccessfully(id), MessageTypeEnum.INFO);
            //}
            //else
            //    m_logging.Log(Messages.FailedExecutingCommand(id), MessageTypeEnum.FAIL);
            if (!result)
            {
                m_logging.Log(Messages.FailedExecutingCommand(id), MessageTypeEnum.FAIL);
            }
        }

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
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    m_logging.Log(Messages.ErrorSendingConfigAndLogDataToClient(e), MessageTypeEnum.FAIL);
                }
                {
                    while (serverIsOn)
                    {
                        try
                        {
                            string requset = reader.ReadString(); // Wait for client to send request
                            CommunicationProtocol msg = JsonConvert.DeserializeObject<CommunicationProtocol>(requset);
                            if (msg != null)
                            {
                                Answer(writer, msg);
                            }
                            else
                            {
                                m_logging.Log(Messages.ErrorRecievingMessageFromClient(), MessageTypeEnum.FAIL);
                            }
                        }
                        catch (Exception e)
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

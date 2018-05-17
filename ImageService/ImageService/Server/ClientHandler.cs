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
        public event clientDelegation CloseClientEvent;
        public Mutex Mutex { get; set; }
        #endregion

        public ClientHandler(IImageController controller, ILoggingService logging,
            IDirectoryHandlerNotifier handlersNotifier)
        {
            m_controller = controller;
            m_logging = logging;
            m_handlersNotifier = handlersNotifier;
        }
        //TODO
        public void DirectoryHandlerIsBeingClosed(object sender, DirectoryCloseEventArgs e)
        {
            // send alarment to the socket about handler that is being closed
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
                writer.Write(configs);
                writer.Write(logs);
            }
        }
        private void Answer(BinaryWriter writer, CommunicationProtocol msg)
        {
            CommandEnum id = (CommandEnum)msg.Command_Id;
            bool result;
            string commandRes = m_controller.ExecuteCommand(msg.Command_Id, msg.Command_Args, out result);
            Mutex.WaitOne();
            writer.Write(commandRes);
            Mutex.ReleaseMutex();

            if (result)
                m_logging.Log(Messages.CommandRanSuccessfully(id), MessageTypeEnum.INFO);
            else
                m_logging.Log(Messages.FailedExecutingCommand(id), MessageTypeEnum.FAIL);
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
                    while (true)
                    {
                        try
                        {
                            string requset = reader.ReadString(); // Wait for client to send request
                            CommunicationProtocol msg = JsonConvert.DeserializeObject<CommunicationProtocol>(requset);
                            if (msg != null)
                            {
                                if (msg.Command_Id == (int)CommandEnum.CloseGUICommand)
                                {
                                    CloseClientEvent?.Invoke(client);
                                    CommunicationProtocol closeClient = new CommunicationProtocol((int)CommandEnum.CloseGUICommand, null);
                                    string closeApprovedString = JsonConvert.SerializeObject(closeClient);
                                    Mutex.WaitOne();
                                    writer.Write(closeApprovedString);
                                    Mutex.ReleaseMutex();
                                }
                                else
                                    Answer(writer, msg);
                            }
                            else
                            {
                                m_logging.Log(Messages.ErrorRecievingMessageFromClient(), MessageTypeEnum.FAIL);
                            }
                        }
                        catch (Exception)
                        {
                            //m_logging.Log(Messages.ErrorHandlingClient(), MessageTypeEnum.FAIL);
                        }
                    }
                }
            }).Start();
        }
    }
}

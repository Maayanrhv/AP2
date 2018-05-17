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


namespace ImageService.Server
{
    class ClientHandler : IClientHandler
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        public Mutex Mutex { get; set; }
        #endregion

        public ClientHandler(IImageController controller, ILoggingService logging)
        {
            m_controller = controller;
            m_logging = logging;
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
                    bool res1, res2;
                    string commandRes1 = m_controller.ExecuteCommand(
                        (int)CommandEnum.GetConfigCommand, null, out res1);
                    string commandRes2 = m_controller.ExecuteCommand(
                        (int)CommandEnum.GetLogCommand, null, out res2);
                    if (res1 && res2)
                    {
                        writer.Write(commandRes1);
                        //writer.Write(commandRes2);
                    }
                } catch (Exception e)
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

                                //if (id == CommandEnum.CloseGUICommand)
                                //{
                                //    ExcludeClient?.Invoke(client);
                                //    CommunicationProtocol closeApproved = new CommunicationProtocol((int)CommandEnum.CloseGUICommand, null);
                                //    string closeApprovedString = JsonConvert.SerializeObject(closeApproved);
                                //    Mutex.WaitOne();
                                //    writer.Write(closeApprovedString);
                                //    Mutex.ReleaseMutex();
                                //}
                                //else if (id == CommandEnum.CloseHandlerCommand)
                                //{
                                //    string[] commandArgs = msg.Command_Args;
                                //    CommandRecievedEventArgs c = new CommandRecievedEventArgs(command, commandArgs);
                                //    CommandRecieved?.Invoke(this, c); // Invoke ImageServer to deal with handler command
                                //}
                                //else
                                //{
                                //    // Not handler command
                                //    bool result;
                                //    string executionResult = c_controller.ExecuteCommand((CommandEnum)msg.CommandID, msg.CommandArgs, out result);
                                //    Mutex.WaitOne();
                                //    writer.Write(executionResult);
                                //    Mutex.ReleaseMutex();

                                //    if (result)
                                //        c_logging.Log($"Command: {command}" + " success", MessageTypeEnum.INFO);
                                //    else
                                //        c_logging.Log($"Command: {command}" + " failed", MessageTypeEnum.FAIL);
                                //}
                            }
                            else
                            {
                                m_logging.Log(Messages.ErrorRecievingMessageFromClient(), MessageTypeEnum.FAIL);
                            }
                        }
                        catch (Exception)
                        {
                            m_logging.Log(Messages.ErrorHandlingClient(), MessageTypeEnum.FAIL);
                        }
                    }
                }
            }).Start();
        }
    }
}

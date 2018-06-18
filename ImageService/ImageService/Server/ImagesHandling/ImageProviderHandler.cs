using ImageService.Infrastructure;
using ImageService.Logging;
using ImageService.Modal.Event;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Server.ImagesHandling
{
    /// <summary>
    /// this class handlers a client- an images provider. 
    /// an images provider only sends msg, and not recieve any, thus this
    /// class only listens to those providers. 
    /// </summary>
    public class ImageProviderHandler
    {
        #region Members
        private ILoggingService m_logging;
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
        public ImageProviderHandler(ILoggingService logging)
        {
            m_logging = logging;
            serverIsOn = true;
        }

        /// <summary>
        /// stop handling all clients. the clients are being close as a result
        /// </summary>
        public void StopHandlingClients()
        {
            serverIsOn = false;
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

                m_logging.Log(Messages.ServerGotNewClientConnection(), MessageTypeEnum.INFO);

                {
                    while (serverIsOn)
                    {
                        try
                        {
                            // Protocall: first- msg size, then- msg.
                            bool clientIsClosed;
                            if (ReadFromClient(reader, out clientIsClosed))
                            {
                                m_logging.Log("read bytes successfully!", MessageTypeEnum.INFO);
                            }
                            else
                            {
                                if (clientIsClosed)
                                    break;
                                m_logging.Log(Messages.ErrorRecievingMessageFromClient(), MessageTypeEnum.FAIL);
                            }
                        }
                        catch (Exception)
                        {
                            m_logging.Log(Messages.ErrorHandlingClient(), MessageTypeEnum.FAIL);
                            break;
                        }
                    }
                    ClientClosedConnectionEvent(this, new ClientEventArgs(client));
                    client.Close();
                }
            }).Start();
        }

        private bool ReadFromClient(BinaryReader reader, out bool clientIsClosed)
        {
            clientIsClosed = false;
            int bytesAmount = reader.ReadInt32();
            if (bytesAmount <= 0)
            {
                clientIsClosed = true;
                return false;
            }
            byte[] picName = reader.ReadBytes(bytesAmount);
            if (picName == null)
                return false;
            bytesAmount = reader.ReadInt32();
            if (bytesAmount <= 0)
            {
                clientIsClosed = true;
                return false;
            }
            byte[] picInBytes = reader.ReadBytes(bytesAmount);
            if (picInBytes == null)
                return false;

            string picNameStr = Encoding.UTF8.GetString(picName);
            HandlePic(picNameStr, picInBytes);
            return true;
        }

        private void HandlePic(string name, byte[] pic)
        {
            //TODO: try cath if there is no handler
            MemoryStream ms = new MemoryStream(pic);
            Image image = Image.FromStream(ms);
            string saveImageIn = ConfigurationManager.AppSettings["Handler"];
            string[] handlers = saveImageIn.Split(';');
            string imgPath = handlers[0] + "\\" + name;
            image.Save(imgPath);
        }
    }
}

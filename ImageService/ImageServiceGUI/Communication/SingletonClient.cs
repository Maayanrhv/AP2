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

namespace ImageServiceGUI.Communication
{
    public sealed class SingletonClient
    {
        #region Members
        private static SingletonClient instance = null;
        TcpClient client;
        bool stop = false;
        private static Mutex mutex = new Mutex();
        #endregion

        public static SingletonClient getInstance
        {
            get
            {
                if (instance == null)
                    instance = new SingletonClient();
                return instance;
            }
        }

        public void DeleteHandler(List<string> handlers)
        {

        }


        public bool connectToServer()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            this.client = new TcpClient();
            try
            {
                client.Connect(ep);
                recieveDataFromServer();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //public void useClient()
        //{
        //    using (NetworkStream stream = this.client.GetStream())
        //    using (BinaryReader reader = new BinaryReader(stream))
        //    using (BinaryWriter writer = new BinaryWriter(stream))
        //    {
        //        // Send data to server
        //        Console.Write("Please enter a number: ");
        //        int num = int.Parse(Console.ReadLine());
        //        writer.Write(num);
        //        // Get result from server
        //        int result = reader.ReadInt32();
        //        Console.WriteLine("Result = {0}", result);
        //    }
        //}

        //public void getDataFromServerUponClientConnection()
        //{
        //    // Translate the passed message into ASCII and store it as a Byte array.
        //    Byte[] data = System.Text.Encoding.ASCII.GetBytes("Hi! msg");
        //    // Get a client stream for reading and writing.
        //    NetworkStream stream = this.client.GetStream();
        //    // Send the message to the connected TcpServer.
        //    stream.Write(data, 0, data.Length);
        //}

        public void sendDataToServer(CommunicationProtocol msg)
        {
            new Task(() =>
            {
                string jsonCommand = JsonConvert.SerializeObject(msg);
                NetworkStream stream = client.GetStream();
                BinaryWriter writer = new BinaryWriter(stream);
                mutex.WaitOne();
                writer.Write(jsonCommand);
                mutex.ReleaseMutex();
            }).Start();
        }

        public void recieveDataFromServer()
        {
            new Task(() =>
            {

                NetworkStream stream = this.client.GetStream();
                BinaryReader reader = new BinaryReader(stream);

                while (!stop)
                {
                    try
                    {
                        string response = reader.ReadString(); // Wait for response from server
                        CommunicationProtocol msg = JsonConvert.DeserializeObject<CommunicationProtocol>(response);
                        Thread.Sleep(1000); // Update information every 1 second
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }).Start();
        }

        public void closeClient()
        {
            this.client.Close();
            this.stop = true;
        }
    }
}
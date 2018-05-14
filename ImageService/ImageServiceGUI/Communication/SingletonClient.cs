using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Communication
{
    public sealed class SingletonClient
    {
        #region Members
        private static SingletonClient instance = null;
        #endregion

        private SingletonClient()
        {
            //this.connectToServer();
        }

        public static SingletonClient getInstance
        {
            get
            {
                if (instance == null)
                    instance = new SingletonClient();
                return instance;
            }
        }

        public void connectToServer()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            TcpClient client = new TcpClient();
            client.Connect(ep);
            using (NetworkStream stream = client.GetStream())
            using (BinaryReader reader = new BinaryReader(stream))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // Send data to server
                Console.Write("Please enter a number: ");
                int num = int.Parse(Console.ReadLine());
                writer.Write(num);
                // Get result from server
                int result = reader.ReadInt32();
                Console.WriteLine("Result = {0}", result);
            }
            client.Close();
        }
    }
}
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
        TcpClient client;
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

        public bool connectToServer()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            this.client = new TcpClient();
            try
            {
                client.Connect(ep);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void useClient()
        {
            using (NetworkStream stream = this.client.GetStream())
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
        }

        public void closeClient()
        {
            this.client.Close();
        }
    }
}
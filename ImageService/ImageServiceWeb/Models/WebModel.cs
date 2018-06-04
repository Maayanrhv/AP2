using ImageService.Communication;
using System.Collections.Generic;


namespace ImageServiceWeb.Models
{
    public class WebModel
    {
        public bool IsServiceConnected
        {
            get;
            private set;
        }

        public SingletonClient Connection
        {
            get;
            private set;
        }

        public Dictionary<string, string> ConfigMap { get; private set; }
        public List<Log> LogsList { get; private set; }


        public WebModel()
        {
            Connection = SingletonClient.getInstance;
            Connection.MsgRecievedFromServer += delegate (object sender, ServiceInfoEventArgs e)
            {
                if (e.ConfigMap != null) { ConfigMap = e.ConfigMap; }
                if (e.LogsList != null) { LogsList = e.LogsList; }
            };
            Connection.ConnectionIsBroken += delegate (object sender, ConnectionArgs args)
            {
                IsServiceConnected = false;
            };
            IsServiceConnected = Connection.ConnectToServer();
        }
    }
}
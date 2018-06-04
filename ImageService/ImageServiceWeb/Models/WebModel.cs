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
                //App.Current.Dispatcher.Invoke((Action)delegate
                //{
                IsServiceConnected = false;
                //});
            };
            IsServiceConnected = Connection.ConnectToServer();
        }

        //private void SetConfigInfo(Dictionary<string, string> config)
        //{
        //    string value;
        //    config.TryGetValue("OutputDir", out value);
        //    this.OutputDirectory = value;
        //    config.TryGetValue("SourceName", out value);
        //    this.SourceName = value;
        //    config.TryGetValue("LogName", out value);
        //    this.LogName = value;
        //    config.TryGetValue("ThumbnailSize", out value);
        //    this.ThumbnailSize = value;
        //    if (config.TryGetValue("Handler", out value))
        //    {
        //        SetHandlers(value.Split(';').ToList<string>());
        //    }
        //}
        //private void SetHandlers(List<string> handlers)
        //{
        //    App.Current.Dispatcher.Invoke((Action)delegate
        //    {
        //        foreach (string handler in handlers)
        //        {
        //            HandlersList.Add(handler);
        //        }
        //    });
        //}

    }
}
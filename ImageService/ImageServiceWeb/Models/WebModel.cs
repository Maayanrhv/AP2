using ImageService.Communication;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ImageServiceWeb.Models
{
    public class WebModel : INotifyPropertyChanged
    {

        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

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

        private Dictionary<string, string> configMap;
        public Dictionary<string, string> ConfigMap
        {
            get
            {
                return configMap;
            }
            private set
            {
                this.configMap = value;
                NotifyPropertyChanged("ConfigMap");
            }
        }
        public List<Log> LogsList { get; private set; }
        public List<string> Handlers { get; private set; }


        public WebModel()
        {
            Handlers = new List<string>();
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


        private void SetConfigInfo(Dictionary<string, string> config)
        {
            string value;
            if (config.TryGetValue("Handler", out value))
            {
                SetHandlers(value.Split(';').ToList<string>());
            }
        }
        private void SetHandlers(List<string> handlers)
        {
            //App.Current.Dispatcher.Invoke((Action)delegate
            //{
                foreach (string handler in handlers)
                {
                    Handlers.Add(handler);
                }
            //});
        }

    }
}
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
        public string HandlerToDelete { get; set; }


        public void UpdateInfoFromServer(object sender, ServiceInfoEventArgs e)
        {
            if (e.ConfigMap != null)
                SetConfigInfo(e.ConfigMap);
            if (e.LogsList != null) {
                LogsList = e.LogsList;
            }
            if (e.RemovedHandlers != null)
            {
                foreach (string handler in e.RemovedHandlers)
                {
                    Handlers.Remove(handler);
                    NotifyPropertyChanged("Deleted:" + handler);
                }

            }
        }

        public WebModel()
        {
            Handlers = new List<string>();
            Connection = SingletonClient.getInstance;
            Connection.MsgRecievedFromServer += UpdateInfoFromServer;
            Connection.ConnectionIsBroken += delegate (object sender, ConnectionArgs args)
            {
                IsServiceConnected = false;
            };
            bool result;
            ServiceInfoEventArgs info = Connection.ConnectToServer(out result);
            if (IsServiceConnected = result)
            {
                UpdateInfoFromServer(this, info);
            }
        }

        private void SetConfigInfo(Dictionary<string, string> config)
        {
            ConfigMap = config;
            string value;
            if (config.TryGetValue("Handler", out value))
            {
                SetHandlers(value.Split(';').ToList<string>());
            }
        }
        private void SetHandlers(List<string> handlers)
        {
                foreach (string handler in handlers)
                {
                    Handlers.Add(handler);
                }
        }
    }
}
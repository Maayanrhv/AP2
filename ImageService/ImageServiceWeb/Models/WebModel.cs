using ImageService.Communication;
using System.Collections.Generic;
using System.Linq;

namespace ImageServiceWeb.Models
{
    public class WebModel : IWebModel
    {
        /// <summary>
        /// holds the service status
        /// </summary>
        public bool IsServiceConnected
        {
            get;
            private set;
        }

        /// <summary>
        /// service's configuraion info 
        /// </summary>
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
            }
        }
        
        /// <summary>
        /// service's logs
        /// </summary>
        public List<Log> LogsList { get; private set; }

        /// <summary>
        /// service's Handlers(Tracked Folders)
        /// </summary>
        public List<string> Handlers { get; private set; }

        /// <summary>
        /// a handler the user chose to delete
        /// </summary>
        public string HandlerToDelete { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        public WebModel()
        {
            Handlers = new List<string>();
            LogsList = new List<Log>();
            SingletonClient connection = SingletonClient.getInstance;
            connection.ConnectionIsBroken += delegate (object sender, ConnectionArgs args)
            {
                IsServiceConnected = false;
            };
            IsServiceConnected = false;
        }

        /// <summary>
        /// this is called when information from the service server needs to be updated in web
        /// </summary>
        /// <param name="sender">who sends this</param>
        /// <param name="e">server info</param>
        private void UpdateInfoFromServer(object sender, ServiceInfoEventArgs e)
        {
            if (e.ConfigMap != null)
                SetConfigInfo(e.ConfigMap);
            if (e.LogsList != null) {
                foreach(Log log in e.LogsList)
                {
                    LogsList.Add(log);
                }
            }
        }

        /// <summary>
        /// opens communication channel with ImageService
        /// </summary>
        public void ConnectToService()
        {
            bool result;
            SingletonClient connection = SingletonClient.getInstance;
            ServiceInfoEventArgs info = connection.ConnectToServer(out result);
            if (IsServiceConnected = result)
            {
                UpdateInfoFromServer(this, info);
            }
        }

        /// <summary>
        /// initialize configuration data in the model.
        /// </summary>
        /// <param name="config">configuration data</param>
        private void SetConfigInfo(Dictionary<string, string> config)
        {
            ConfigMap = config;
            string value;
            if (config.TryGetValue("Handler", out value))
            {
                SetHandlers(value.Split(';').ToList<string>());
            }
        }

        /// <summary>
        /// fills handlers list
        /// </summary>
        /// <param name="handlers">tracked folders paths</param>
        private void SetHandlers(List<string> handlers)
        {
                foreach (string handler in handlers)
                {
                    Handlers.Add(handler);
                }
        }

        /// <summary>
        /// sends a request for deletion of a handler and waits for answer from server.
        /// </summary>
        public void CloseHandler()
        {
            SingletonClient connection = SingletonClient.getInstance;
            ServiceInfoEventArgs answer = connection.CloseHandler(new List<string>() { HandlerToDelete });
            if (answer.RemovedHandlers.Contains(HandlerToDelete))
                Handlers.Remove(HandlerToDelete);
        }
    }
}
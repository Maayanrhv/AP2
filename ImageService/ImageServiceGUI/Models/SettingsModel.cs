using ImageService.Communication;
using ImageServiceGUI.Communication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ImageServiceGUI.Models
{
    /// <summary>
    /// responsible of config & handlers data
    /// </summary>
    public class SettingsModel : ISettingsModel
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        public SettingsModel()
        {
            SingletonClient client = SingletonClient.getInstance;
            client.MsgRecievedFromServer += delegate(object sender, ServiceInfoEventArgs e)
            {
                if (e.ConfigMap != null) { SetConfigInfo(e.ConfigMap); }
                if (e.RemovedHandlers != null)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        foreach (string handler in e.RemovedHandlers)
                            this.HandlersList.Remove(handler);
                    });
                }
            };
            this.HandlersList = new ObservableCollection<string>();
        }

        /// <summary>
        /// initialize the config properties with the config Dictionary given.
        /// </summary>
        /// <param name="config">App.config information</param>
        private void SetConfigInfo(Dictionary<string, string> config)
        {
            string value;
            config.TryGetValue("OutputDir", out value);
            this.OutputDirectory = value;
            config.TryGetValue("SourceName", out value);
            this.SourceName = value;
            config.TryGetValue("LogName", out value);
            this.LogName = value;
            config.TryGetValue("ThumbnailSize", out value);
            this.ThumbnailSize = value;
            if (config.TryGetValue("Handler", out value))
            {
                SetHandlers(value.Split(';').ToList<string>());
            }
        }

        /// <summary>
        /// HandlersList management
        /// initialize the handlers list with the given handlers.
        /// </summary>
        /// <param name="handlers">handlers in config</param>
        private void SetHandlers(List<string> handlers)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach (string handler in handlers)
                {
                    HandlersList.Add(handler);
                }
            });
        }

        #region Config Properties
        private string m_thumbnailSize;
        public string ThumbnailSize
        {
            get { return m_thumbnailSize; }
            set
            {
                m_thumbnailSize = value;
                NotifyPropertyChanged("ThumbnailSize");
            }
        }

        private string m_logName;
        public string LogName
        {
            get { return m_logName; }
            set
            {
                m_logName = value;
                NotifyPropertyChanged("LogName");
            }
        }

        private string m_sourceName;
        public string SourceName
        {
            get { return m_sourceName; }
            set
            {
                m_sourceName = value;
                NotifyPropertyChanged("SourceName");
            }
        }

        private string m_outputDirectory;
        public string OutputDirectory
        {
            get { return m_outputDirectory; }
            set
            {
                m_outputDirectory = value;
                NotifyPropertyChanged("OutputDirectory");
            }
        }
        #endregion

        #region Handlers Properties
        private string m_chosenHandler;
        public string ChosenHandler
        {
            get { return m_chosenHandler; }
            set
            {
                m_chosenHandler = value;
                NotifyPropertyChanged("ChosenHandler");

            }
        }

        public ObservableCollection<string> HandlersList { get; private set; }
        #endregion

        /// <summary>
        /// send a request to remove a directory handler from Service.
        /// </summary>
        /// <param name="handler">a handler to remove</param>
        public void RemoveHandler(string handler)
        {
            SingletonClient client = SingletonClient.getInstance;
            List<string> l = new List<string>();
            l.Add(handler);
            client.CloseHandler(l);
        }
    }
}

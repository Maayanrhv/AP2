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

namespace ImageServiceGUI.Models
{
    public class SettingsModel
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public SettingsModel()
        {
            SingletonClient client = SingletonClient.getInstance;
            //client.newMsgRecieved+=...event handler method

            this.HandlersList = GetHandlers();
            SetConfigInfo();
        }

        private void SetConfigInfo()
        {
            this.OutputDirectory = "path to some dir";
            this.SourceName = "some source name";
            this.LogName = "loglogloglog";
            this.ThumbnailSize = "300000";
        }

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

        public ObservableCollection<string> HandlersList { get; private set; }

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

        // HandlersList management
        private ObservableCollection<string> GetHandlers()
        {

            ObservableCollection<string> handlersList = new ObservableCollection<string>() { "give me 'HAND'!", "HAND!", "give me 'LER'!", "LER!", "What came out??", "HANDLER!!"};
            return handlersList;
        }

        public bool RemoveHandler(string handler)
        {
            bool result = true;
            SingletonClient client = SingletonClient.getInstance;

            //if (result)
            //{
            //    this.HandlersList.Remove(handler);
            //}
            return result;
        }

        public void MMM(object sender)
        {
            // if e.
        }

        
    }
}

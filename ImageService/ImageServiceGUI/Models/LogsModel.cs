using ImageService.Communication;
using ImageService.Logging;
using ImageServiceGUI.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using static ImageService.Logging.MessageTypeEnum;

namespace ImageServiceGUI.Models
{

    public class LogsModel
    {
        private static Mutex mutex = new Mutex();

        public DataTable dt { get; private set; }

        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public LogsModel()
        {
            SingletonClient client = SingletonClient.getInstance;
            client.MsgRecievedFromServer += MsgFromServer;
        }
        public void MsgFromServer(object sender, ServiceInfoEventArgs e)
        {
            if (e.logs_List!= null)
            {
                AddLogs(e.logs_List);
            }
        }

        private void AddLogs(List<Couple> logs)
        {
            mutex.WaitOne();
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach (Couple log in logs)
                {
                    this.LogRecentelyAdded = new Couple(log.Type, log.Log);
                }
            });
            mutex.ReleaseMutex();
        }

        private Couple m_logRecentelyAdded;
        public Couple LogRecentelyAdded
        {
            get { return m_logRecentelyAdded; }
            set
            {
                m_logRecentelyAdded = value;
                NotifyPropertyChanged("LogRecentelyAdded");
            }
        }

    }

}

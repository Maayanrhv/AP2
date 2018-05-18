using GUI.Models;
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
    public class LogsModel : ILogsModel
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
            dt = new DataTable();
            SetDT();

        }
        private void SetDT()
        {
            DataColumn type = new DataColumn("Type", typeof(string));
            DataColumn message = new DataColumn("Message", typeof(string));
            
            dt.Columns.Add(type);
            dt.Columns.Add(message);
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
                    AddLog(log);
                }
            });
            mutex.ReleaseMutex();
        }

        private void AddLog(Couple p)
        {
            string t = p.Type.ToString();
            DataRow r = dt.NewRow();
            r[0] = t;
            r[1] = p.Log;
            dt.Rows.InsertAt(r, 0);
        }
    }

}

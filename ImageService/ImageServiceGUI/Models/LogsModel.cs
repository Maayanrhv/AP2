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
    /// <summary>
    /// responsible of logs data
    /// </summary>
    public class LogsModel : ILogsModel
    {
        private static Mutex mutex = new Mutex();

        /// <summary>
        /// table of logs - changes dynamically. 
        /// </summary>
        public DataTable dt { get; private set; }

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
        public LogsModel()
        {
            SingletonClient client = SingletonClient.getInstance;
            client.MsgRecievedFromServer += delegate (object sender, ServiceInfoEventArgs e)
            {
                if (e.LogsList != null)
                {
                    AddLogs(e.LogsList);
                }
            };
            dt = new DataTable();
            SetDT();

        }

        /// <summary>
        /// initialize the logs table
        /// </summary>
        private void SetDT()
        {
            DataColumn type = new DataColumn("Type", typeof(string));
            DataColumn message = new DataColumn("Message", typeof(string));
            
            dt.Columns.Add(type);
            dt.Columns.Add(message);
        }

        /// <summary>
        /// adding logs to logs table
        /// </summary>
        /// <param name="logs">the logs to add</param>
        private void AddLogs(List<Log> logs)
        {
            mutex.WaitOne();
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach (Log log in logs)
                {
                    AddLog(log);
                }
            });
            mutex.ReleaseMutex();
        }

        /// <summary>
        /// adding a log to the logs table
        /// </summary>
        /// <param name="log">the log to add</param>
        private void AddLog(Log log)
        {
            string type = log.Type.ToString();
            DataRow r = dt.NewRow();
            r[0] = type;
            r[1] = log.Content;
            dt.Rows.InsertAt(r, 0);
        }
    }

}
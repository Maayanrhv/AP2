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
using System.Threading.Tasks;
using System.Windows.Media;
using static ImageService.Logging.MessageTypeEnum;

namespace ImageServiceGUI.Models
{

    public class LogsModel
    {
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
            dt = new DataTable();
            SetDT();
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
            foreach(Couple log in logs)
            {
                AddLog(log.Type, log.Log);
            }
        }

        private void SetDT()
        {
            DataColumn type = new DataColumn("Type", typeof(string));
            DataColumn message = new DataColumn("Message", typeof(string));
            dt.Columns.Add(type);
            dt.Columns.Add(message);
        }

        public void AddLog(MessageTypeEnum type, string msg)
        {
            string t = type.ToString();
            DataRow r = dt.NewRow();
            r[0] = t;

            r[1] = msg;
            dt.Rows.InsertAt(r, 0);

            //dt.Rows[0].Cells[0].Style.BackColor = Color.Red;
        }

        private void getItem()
        {
            string g;
            g = Console.ReadLine();
        }
    }

}

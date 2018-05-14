using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Models
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


        }

        private void SetDT()
        {
            DataColumn type = new DataColumn("Type", typeof(string));
            DataColumn message = new DataColumn("Message", typeof(string));

            dt.Columns.Add(type);
            dt.Columns.Add(message);

            AddLog("ERROR", "logilog");
            AddLog("INFO", "I log youuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuuu");

            
        }

        public void AddLog(string type, string msg)
        {
            DataRow r = dt.NewRow();
            r[0] = type;
            r[1] = msg;
            dt.Rows.InsertAt(r, 0);
        }

        private void getItem()
        {
            string g;
            g = Console.ReadLine();
        }
    }

}

using ImageService.Communication;
using ImageServiceGUI.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageServiceGUI.ViewModels
{
    public class LogsViewModel
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public DataTable dt { get; private set; }

        public LogsViewModel()
        {
            this.m_logsModel = new LogsModel();
            m_logsModel.PropertyChanged +=
                    delegate (Object sender, PropertyChangedEventArgs e)
                    {
                        if (e.PropertyName == "LogRecentelyAdded")
                        {
                            Couple p = m_logsModel.LogRecentelyAdded;
                            string t = p.Type.ToString();
                            DataRow r = dt.NewRow();
                            r[0] = t;
                            r[1] = p.Log;
                            dt.Rows.InsertAt(r, 0);
                        }
                        NotifyPropertyChanged(e.PropertyName);
                    };
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




        private LogsModel m_logsModel;
        public LogsModel LogsModel
        {
            get { return this.m_logsModel; }
            set
            {
                this.m_logsModel = value;
            }
        }


        //#region button check        
        //public ICommand RemoveCommand { get; private set; }

        //private void OnRemove(object obj)
        //{
        //    //LogsModel.AddLog(ImageService.Logging.MessageTypeEnum.FAIL, "new log is in the hous!");
        //}

        //private bool CanRemove(object obj)
        //{
        //    return true;
        //}
        //this.RemoveCommand = new DelegateCommand<object>(this.OnRemove, this.CanRemove);

        //#endregion

    }


}

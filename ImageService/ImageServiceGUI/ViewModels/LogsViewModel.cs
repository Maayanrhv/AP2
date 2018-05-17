using GUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GUI.ViewModels
{
    public class LogsViewModel
    {
        public LogsViewModel()
        {
            this.m_logsModel = new LogsModel();


         

            //TODO: to not be able to click a row



            //this.PropertyChanged += RemoveButtonHandler;
            //this.sm = new SettingsModel();
            //sm.PropertyChanged +=
            //   delegate (Object sender, PropertyChangedEventArgs e) {
            //       NotifyPropertyChanged(e.PropertyName);
            //   };
            //this.RemoveCommand = new DelegateCommand<object>(this.OnRemove, this.CanRemove);

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







    }


}

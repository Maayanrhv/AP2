using GUI.Models;
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
    public class LogsViewModel : INotifyPropertyChanged
    {
        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public LogsViewModel(ILogsModel logsModel)
        {
            this.m_logsModel = logsModel;
            m_logsModel.PropertyChanged +=
                    delegate (Object sender, PropertyChangedEventArgs e)
                    {
                        NotifyPropertyChanged(e.PropertyName);
                    };
        }

        private ILogsModel m_logsModel;
        public ILogsModel LogsModel
        {
            get { return this.m_logsModel; }
            set
            {
                this.m_logsModel = value;
            }
        }
    }


}

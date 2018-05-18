using ImageServiceGUI.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;


namespace ImageServiceGUI.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private ISettingsModel m_settingsModel;
        public ISettingsModel SettingsModel
        {
            get { return this.m_settingsModel; }
            set
            {
                this.m_settingsModel = value;
            }
        }

        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        protected void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public SettingsViewModel(ISettingsModel SettingsModel)
        {
            this.PropertyChanged += RemoveButtonHandler;
            this.m_settingsModel = SettingsModel;
            m_settingsModel.PropertyChanged +=
               delegate (Object sender, PropertyChangedEventArgs e) {
                   NotifyPropertyChanged(e.PropertyName);
            };
            this.RemoveCommand = new DelegateCommand<object>(this.OnRemove, this.CanRemove);
        }

        //#region properties
        //private string m_outputDirectory;
        //public string OutputDirectory
        //{
        //    get { return m_outputDirectory; }
        //    set
        //    {
        //        m_outputDirectory = value;
        //        NotifyPropertyChanged("OutputDirectory");
        //    }
        //}

        //private string m_chosenHandler;
        //public string ChosenHandler
        //{
        //    get { return m_chosenHandler; }
        //    set
        //    {
        //        m_chosenHandler = value;
        //        NotifyPropertyChanged("ChosenHandler");

        //    }
        //}

        //private string m_thumbnailSize;
        //public string ThumbnailSize
        //{
        //    get { return m_thumbnailSize; }
        //    set
        //    {
        //        m_thumbnailSize = value;
        //        NotifyPropertyChanged("ThumbnailSize");
        //    }
        //}

        //private string m_logName;
        //public string LogName
        //{
        //    get { return m_logName; }
        //    set
        //    {
        //        m_logName = value;
        //        NotifyPropertyChanged("LogName");
        //    }
        //}

        //private string m_sourceName;
        //public string SourceName
        //{
        //    get { return m_sourceName; }
        //    set
        //    {
        //        m_sourceName = value;
        //        NotifyPropertyChanged("SourceName");
        //    }
        //}
        //#endregion

        // Remove button handling
        private void RemoveButtonHandler(object sender, PropertyChangedEventArgs e)
        {
            var command = this.RemoveCommand as DelegateCommand<object>;
            command.RaiseCanExecuteChanged();
        }

        public ICommand RemoveCommand { get; private set; }

        private void OnRemove(object obj)
        {
            this.SettingsModel.RemoveHandler(SettingsModel.ChosenHandler);
        }

        private bool CanRemove(object obj)
        {
            if (string.IsNullOrEmpty(SettingsModel.ChosenHandler))
            {
                return false;
            }
            return true;
        }

        //private string BuildResultString()
        //{
        //    StringBuilder builder = new StringBuilder();
        //    // TODO: need to add all checked
        //    builder.Append(SettingsModel.ChosenHandler);
        //    //foreach(string str in TempList) {

        //    //    builder.Append(str + "\n");
        //    //}
        //    return builder.ToString();
        //}
    }
}

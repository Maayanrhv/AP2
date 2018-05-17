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
        private SettingsModel m_settingsModel;
        public SettingsModel SettingsModel
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

        public SettingsViewModel()
        {
            this.PropertyChanged += RemoveButtonHandler;
            this.m_settingsModel = new SettingsModel();
            m_settingsModel.PropertyChanged +=
               delegate (Object sender, PropertyChangedEventArgs e) {
                   if (e.PropertyName == "OutputDirectory")
                       this.OutputDirectory = m_settingsModel.OutputDirectory;
                   if (e.PropertyName == "ChosenHandler")
                       this.ChosenHandler = m_settingsModel.ChosenHandler;
                   if (e.PropertyName == "ThumbnailSize")
                       this.ThumbnailSize = m_settingsModel.ThumbnailSize;
                   if (e.PropertyName == "LogName")
                       this.LogName = m_settingsModel.LogName;
                   if (e.PropertyName == "SourceName")
                       this.SourceName = m_settingsModel.SourceName;


                   NotifyPropertyChanged(e.PropertyName);
            };
            this.RemoveCommand = new DelegateCommand<object>(this.OnRemove, this.CanRemove);
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


        // Remove button handling
        private void RemoveButtonHandler(object sender, PropertyChangedEventArgs e)
        {
            var command = this.RemoveCommand as DelegateCommand<object>;
            command.RaiseCanExecuteChanged();
        }

        public ICommand RemoveCommand { get; private set; }

        private void OnRemove(object obj)
        {
            Debug.WriteLine("trying to remove handler: " + this.BuildResultString());
            bool result = this.SettingsModel.RemoveHandler(SettingsModel.ChosenHandler);
            if (result)
            {
                Debug.WriteLine("handler was removed successfuly");
            } else
            {
                Debug.WriteLine("failed to remove handler");
            }
        }

        private bool CanRemove(object obj)
        {
            if (string.IsNullOrEmpty(SettingsModel.ChosenHandler))
            {
                return false;
            }
            return true;
        }

        private string BuildResultString()
        {
            StringBuilder builder = new StringBuilder();
            // TODO: need to add all checked
            builder.Append(SettingsModel.ChosenHandler);
            //foreach(string str in TempList) {

            //    builder.Append(str + "\n");
            //}
            return builder.ToString();
        }
    }
}

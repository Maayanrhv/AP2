using ImageServiceGUI.ViewModels;
using ImageServiceGUI.Communication;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GUI.Communication;
using ImageServiceGUI.Models;
using Prism.Commands;

namespace ImageServiceGUI.ViewModels
{
    /// <summary>
    /// responsible for connecting to Settings & Logs, for tab control, for closing the 
    /// window and for determine Server connection.
    /// </summary>
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel SettingsViewModel { get; set; }
        public LogsViewModel LogsViewModel { get; set; }
        private string serverIsOffColor;

        #region Notify Changed
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName) {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        public MainWindowViewModel()
        {
            serverIsOffColor = "gray";
            SingletonClient client = SingletonClient.getInstance;
            client.ConnectionIsBroken += delegate (object sender, ConnectionArgs args)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.BackgroundColor = serverIsOffColor;
                });
            };
            // SettingsModel and LogsModel register at the client's event
            this.SettingsViewModel = new SettingsViewModel(new SettingsModel());
            this.LogsViewModel = new LogsViewModel(new LogsModel());

            if (!ConnectToServer())
            {
                this.BackgroundColor = serverIsOffColor;
            }
            CloseWindowCommand = new DelegateCommand<object>(OnClose, CanClose);
        }

        /// <summary>
        /// tabs backgroundColor property
        /// </summary>
        private string m_backgroundColor;
        public string BackgroundColor
        {
            get { return m_backgroundColor; }
            set
            {
                m_backgroundColor = value;
                NotifyPropertyChanged("BackgroundColor");
            }
        }

        /// <summary>
        /// connect the GUI to a Server
        /// </summary>
        /// <returns>true if Succeeded to connect, false o.w</returns>
        private bool ConnectToServer()
        {
            SingletonClient client = SingletonClient.getInstance;
            bool result = client.ConnectToServer();
            return result;
        }

        #region Closing Window Handling
        /// <summary>
        /// closing the window command
        /// </summary>
        public ICommand CloseWindowCommand { get; private set; }
        
        /// <summary>
        /// when can the window be closed - always
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>if the window is allowed to be closed</returns>
        private bool CanClose(object arg)
        {
            return true; // Allowing the user to close the window always
        }

        /// <summary>
        /// what to do when the user wants to close the window
        /// </summary>
        /// <param name="obj"></param>
        private void OnClose(object obj)
        {
            SingletonClient client = SingletonClient.getInstance;
            client.StartClosingWindow();
        }
        #endregion
    }
}

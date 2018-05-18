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
using ImageServiceGUI.Models;


namespace ImageServiceGUI.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel SettingsViewModel { get; set; }
        public LogsViewModel LogsViewModel { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName) {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        public ICommand CloseCommand { get; private set; }
        private MainWindowModel mainWindowModel;

        public MainWindowViewModel()
        {
            SingletonClient client = SingletonClient.getInstance;
            client.ConnectionIsBroken += delegate (object sender, ConnectionArgs args)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.BackgroundColor = "gray";
                });
            };
            this.SettingsViewModel = new SettingsViewModel(new SettingsModel());
            this.LogsViewModel = new LogsViewModel(new LogsModel());
            if (!ConnectToServer(client))
            {
                this.BackgroundColor = "gray";
            }

            CloseCommand = new DelegateCommand<object>(OnClose, CanClose);
            mainWindowModel = new MainWindowModel();
        }

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

        private bool ConnectToServer(SingletonClient client)
        {
            bool result = client.connectToServer();
            return result;
        }

        private bool CanClose(object arg)
        {
            return true; // Allowing the user to close the window always
        }

        private void OnClose(object obj)
        {
            mainWindowModel.client.startClosingWindow();
        }
    }
}

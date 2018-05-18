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
using Prism.Commands;
using ImageServiceGUI.Models;


//using System.Diagnostics;
//using System.Text;
//using System.Windows.Input;
//using BasicMVVMQuickstart_Desktop.Model;
//using System.ComponentModel;
//using Microsoft.Practices.Prism.Commands;


namespace ImageServiceGUI.ViewModels
{
    class MainWindowViewModel
    {
        public SettingsViewModel SettingsViewModel { get; set; }
        public LogsViewModel LogsViewModel { get; set; }
        public ICommand CloseCommand { get; private set; }
        private MainWindowModel mainWindowModel;

        public MainWindowViewModel()
        {
            Debug.WriteLine(" ");
            Debug.WriteLine(" ");
            Debug.WriteLine(" ");
            Debug.WriteLine(" ");

            Debug.WriteLine("INIT");

            Debug.WriteLine(" ");
            Debug.WriteLine(" ");
            Debug.WriteLine(" ");
            Debug.WriteLine(" ");
            SingletonClient client = SingletonClient.getInstance;
            SettingsViewModel = new SettingsViewModel();
            LogsViewModel = new LogsViewModel();
            if (ConnectToServer(client))
            {
            } else
            {
                this.BackgroundColor = "gray";
                //TODO: change color
                //TODO: Style to "Settings" head (and Settings head itself)
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

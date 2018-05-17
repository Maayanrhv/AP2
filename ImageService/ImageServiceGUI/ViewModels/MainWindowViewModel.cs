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
            this.SettingsViewModel = new SettingsViewModel();
            this.LogsViewModel = new LogsViewModel();
            if (ConnectToServer(client))
            {
            } else
            {
                this.BackgroundColor = "gray";
                //TODO: change color
                //TODO: Style to "Settings" head (and Settings head itself)
            }
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
        
        public void WindowClosing
        {

        }

    }
}

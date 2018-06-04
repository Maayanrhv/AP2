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
    /// <summary>
    /// connects SettingsView with SettingsModel  
    /// </summary>
    public class SettingsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// the model behind
        /// </summary>
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

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="SettingsModel">the model that respponsible for the config &
        /// handlers information</param>
        public SettingsViewModel(ISettingsModel SettingsModel)
        {
            this.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e) {
                if (e.PropertyName == "ChosenHandler")
                {
                    var command = this.RemoveCommand as DelegateCommand<object>;
                    command.RaiseCanExecuteChanged();
                }
            };
            this.m_settingsModel = SettingsModel;
            m_settingsModel.PropertyChanged +=
               delegate (Object sender, PropertyChangedEventArgs e) {
                   NotifyPropertyChanged(e.PropertyName);
            };
            this.RemoveCommand = new DelegateCommand<object>(this.OnRemove, this.CanRemove);
        }


        #region "Remove Handler" button handling
        /// <summary>
        /// decides if it is possible to press the remove button. and if it is-
        /// than what to do.
        /// </summary>
        public ICommand RemoveCommand { get; private set; }

        /// <summary>
        /// in action if the button "Remove" (handler) was pressed
        /// </summary>
        /// <param name="obj"></param>
        private void OnRemove(object obj)
        {
            this.SettingsModel.RemoveHandler(SettingsModel.ChosenHandler);
        }

        /// <summary>
        /// determines when can the button "Remove" (handler) be pressed.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if the button "Remove" (handler) be pressed. false o.w</returns>
        private bool CanRemove(object obj)
        {
            if (string.IsNullOrEmpty(SettingsModel.ChosenHandler))
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}

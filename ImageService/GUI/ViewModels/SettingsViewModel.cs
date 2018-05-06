using GUI.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;





namespace GUI.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private SettingsModel sm;
        public SettingsModel SettingsModel
        {
            get { return this.sm; }
            set
            {
                this.sm = value;
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
            this.sm = new SettingsModel();
            sm.PropertyChanged +=
               delegate (Object sender, PropertyChangedEventArgs e) {
               NotifyPropertyChanged(e.PropertyName);
            };
            this.RemoveCommand = new DelegateCommand<object>(this.OnRemove, this.CanRemove);
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

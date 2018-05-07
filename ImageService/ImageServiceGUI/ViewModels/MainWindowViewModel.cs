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
            if (ConnectToServer())
            {
                this.SettingsViewModel = new SettingsViewModel();
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

        private bool ConnectToServer()
        {
            bool result = true;
            return result;
        }

        //private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    //var command = this.SubmitCommand as DelegateCommand<object>;
        //    //command.RaiseCanExecuteChanged();
        //}

        //public ICommand SubmitCommand { get; private set; }

        //public ICommand ResetCommand { get; private set; }



        //private void OnSubmit(object obj)
        //{
        //    Debug.WriteLine(this.BuildResultString());
        //}

        //private bool CanSubmit(object obj)
        //{
        //    if (string.IsNullOrEmpty(this.QuestionnaireViewModel.Questionnaire.Name))
        //    {
        //        return false;
        //    }
        //    if (this.QuestionnaireViewModel.Questionnaire.Age < 0 || this.QuestionnaireViewModel.Questionnaire.Age > 120)
        //    {
        //        return false;
        //    }
        //    if (string.IsNullOrEmpty(this.QuestionnaireViewModel.Questionnaire.Quest))
        //    {
        //        return false;
        //    }
        //    if (string.IsNullOrEmpty(this.QuestionnaireViewModel.Questionnaire.FavoriteColor))
        //    {
        //        return false;
        //    }
        //    return true;
        //}


        //private void OnReset()
        //{
        //    this.QuestionnaireViewModel.Questionnaire = new Questionnaire();
        //}

        //private string BuildResultString()
        //{
        //    StringBuilder builder = new StringBuilder();
        //    builder.Append("Name: ");
        //    builder.Append(this.QuestionnaireViewModel.Questionnaire.Name);
        //    builder.Append("\nAge: ");
        //    builder.Append(this.QuestionnaireViewModel.Questionnaire.Age);
        //    builder.Append("\nQuestion 1: ");
        //    builder.Append(this.QuestionnaireViewModel.Questionnaire.Quest);
        //    builder.Append("\nQuestion 2: ");
        //    builder.Append(this.QuestionnaireViewModel.Questionnaire.FavoriteColor);
        //    return builder.ToString();
        //}

    }
}

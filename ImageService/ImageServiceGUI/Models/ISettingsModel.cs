using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    public interface ISettingsModel : INotifyPropertyChanged
    {
        void RemoveHandler(string handler);

        //event PropertyChangedEventHandler PropertyChanged;
        string ThumbnailSize { get; set; }
        string LogName { get; set; }
        string SourceName { get; set; }
        string OutputDirectory { get; set; }
        string ChosenHandler { get; set; }

        ObservableCollection<string> HandlersList { get; }
    }
}

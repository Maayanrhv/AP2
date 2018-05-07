using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    public interface ISettingsModel
    {
        bool RemoveHandler(string handler);
        //string ChosenHandler;
        event PropertyChangedEventHandler PropertyChanged;
        //string ThumbnailSize;
        //string LogName
        //string SourceName
        //string OutputDirectory

        // ObservableCollection<string> HandlersList { get; private set; }



    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    /// <summary>
    /// responsible of config & handlers data
    /// </summary>
    public interface ISettingsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// send a request to remove a directory handler from Service.
        /// </summary>
        /// <param name="handler">a handler to remove</param>
        void RemoveHandler(string handler);

        #region Getters & Setters
        string ThumbnailSize { get; set; }
        string LogName { get; set; }
        string SourceName { get; set; }
        string OutputDirectory { get; set; }
        string ChosenHandler { get; set; }
        #endregion

        /// <summary>
        /// handlers list - changing dynamically
        /// </summary>
        ObservableCollection<string> HandlersList { get; }
    }
}

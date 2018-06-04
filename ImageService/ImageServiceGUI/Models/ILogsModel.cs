using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Models
{
    /// <summary>
    /// responsible of logs data
    /// </summary>
    public interface ILogsModel : INotifyPropertyChanged
    {
        /// <summary>
        /// table of logs
        /// </summary>
        DataTable dt { get; }
    }
}

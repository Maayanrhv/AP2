using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Models
{
    public interface ILogsModel : INotifyPropertyChanged
    {
        DataTable dt { get; }
    }
}

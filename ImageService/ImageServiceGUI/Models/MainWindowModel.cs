using ImageServiceGUI.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageServiceGUI.Models
{
    public class MainWindowModel
    {
        public SingletonClient client { get; set; }

        public MainWindowModel()
        {
            client = SingletonClient.getInstance;
        }
    }
}

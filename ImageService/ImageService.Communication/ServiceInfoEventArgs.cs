using ImageService.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    public class ServiceInfoEventArgs : EventArgs
    {
        private Dictionary<MessageTypeEnum, string> logsMap;
        private Dictionary<string, string> configMap;
        private List<string> removedHandlers;
        #region Notify Changed
        public event EventHandler<ServiceInfoEventArgs> MsgRecievedFromServer;
        #endregion

        // Getters & Setters
        public Dictionary<MessageTypeEnum, string> logs_Map { get; set; }
        public Dictionary<string, string> config_Map { get; set; }
        public List<string> removed_Handlers { get; set; }

    }
}

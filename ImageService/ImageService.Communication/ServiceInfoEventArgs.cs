using ImageService.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Communication
{
    public class Couple
    {
        public Couple(MessageTypeEnum type, string log)
        {
            Type = type;
            Log = log;
        }
        public MessageTypeEnum Type
        {
            get;
            set;
        }
        public string Log
        {
            get;
            set;
        }

    }
    public class ServiceInfoEventArgs : EventArgs
    {
        private List<Couple> logsList;
        private Dictionary<string, string> configMap;
        private List<string> removedHandlers;
        

        // Getters & Setters
        public List<Couple> logs_List { get; set; }
        public Dictionary<string, string> config_Map { get; set; }
        public List<string> removed_Handlers { get; set; }
    }
}
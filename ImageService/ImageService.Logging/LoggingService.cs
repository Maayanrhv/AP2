using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        private event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        public void AddEvent(EventHandler<MessageRecievedEventArgs> func)
        {
            MessageRecieved += new System.EventHandler<MessageRecievedEventArgs>(func);
        }

        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecieved.Invoke(this, new MessageRecievedEventArgs(message,type));
        }
    }
}
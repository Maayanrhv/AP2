﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public interface ILoggingService
    {
        void Log(string message, MessageTypeEnum type);

        void AddEvent(EventHandler<MessageRecievedEventArgs> func);
    }
}
using System;

namespace ImageService.Logging
{
    /// <summary>
    /// arguments for event handlers in ILoggingService's event.
    /// </summary>
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="msg">message content</param>
        /// <param name="type">type of message</param>
        public MessageRecievedEventArgs(string msg, MessageTypeEnum type)
        {
            Message = msg;
            Status = type;
        }
    }
}
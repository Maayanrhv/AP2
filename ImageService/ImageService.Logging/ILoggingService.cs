using System;

namespace ImageService.Logging
{
    /// <summary>
    /// shows messages that were sent from the program on the service log.
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// shows a message on the log
        /// </summary>
        /// <param name="message">message content</param>
        /// <param name="type">type of the message</param>
        void Log(string message, MessageTypeEnum type);

        /// <summary>
        /// adds an event handler that is being called when a new message arrives.
        /// </summary>
        /// <param name="func">event handler: function to be called when the event is invoked</param>
        void AddEvent(EventHandler<MessageRecievedEventArgs> func);
    }
}
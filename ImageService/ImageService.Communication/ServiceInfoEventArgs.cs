using ImageService.Logging;
using System;
using System.Collections.Generic;

namespace ImageService.Communication
{
    /// <summary>
    /// container to log & logType
    /// </summary>
    public class Log
    {
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="type">the type of the log</param>
        /// <param name="log">log</param>
        public Log(MessageTypeEnum type, string log)
        {
            Type = type;
            Content = log;
        }
        #region Properties
        public MessageTypeEnum Type
        {
            get;
            set;
        }
        public string Content
        {
            get;
            set;
        }
        #endregion
    }

    /// <summary>
    /// container for new or changed Service information.
    /// </summary>
    public class ServiceInfoEventArgs : EventArgs
    {
        // Getters & Setters
        /// <summary>
        /// Servic's new logs.
        /// </summary>
        public List<Log> LogsList { get; set; }
        /// <summary>
        /// Servic's App.config content.
        /// </summary>
        public Dictionary<string, string> ConfigMap { get; set; }
        /// <summary>
        /// handlers that had been removed from Service
        /// </summary>
        public List<string> RemovedHandlers { get; set; }
    }
}
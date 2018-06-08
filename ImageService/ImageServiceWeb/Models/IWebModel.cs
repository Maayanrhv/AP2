using ImageService.Communication;
using System.Collections.Generic;

namespace ImageServiceWeb.Models
{
    interface IWebModel
    {
        /// <summary>
        /// service's configuraion info 
        /// </summary>
        Dictionary<string, string> ConfigMap { get; }

        /// <summary>
        /// holds the service status
        /// </summary>
        bool IsServiceConnected { get; }

        /// <summary>
        /// service's Handlers(Tracked Folders)
        /// </summary>
        List<string> Handlers { get; }

        /// <summary>
        /// a handler the user chose to delete
        /// </summary>
        string HandlerToDelete { get; set; }

        /// <summary>
        /// service's logs
        /// </summary>
        List<Log> LogsList { get; }

        /// <summary>
        /// sends a request for deletion of a handler and waits for answer from server.
        /// </summary>
        void CloseHandler();

        /// <summary>
        /// opens communication channel with ImageService
        /// </summary>
        void ConnectToService();
    }
}

using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using System.Text.RegularExpressions;
using ImageService.Modal.Event;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        public DirectoyHandler(IImageController controller, ILoggingService logging)
        {
            this.m_controller = controller;
            this.m_logging = logging;
        }

        // Implement Here!
        public void StartHandleDirectory(string dirPath)
        {

        }
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {

        }
    }
}
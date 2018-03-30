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

        public void StartHandleDirectory(string dirPath)
        {
            m_path = dirPath;
            m_dirWatcher = new FileSystemWatcher(m_path, "*.*");
            m_dirWatcher.Changed += new FileSystemEventHandler(OnChanged);
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // get the file's extension 
            //string strFileExt = getFileExt(e.FullPath);
            // filter file types 
            if(Regex.IsMatch(strFileExt, @"\.jpg)|\.png|\.gif|\.bmp", RegexOptions.IgnoreCase))
            {
                
                // if the file has closed: DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(DirPath, msg))
            }
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {

            //check if e.CommandID == closeHandler command (to ALL handlers)
            //if so - stop watching
            //else - check if command is relevant by comparing paths
            //if relevant - call command by controller
            //controller.execute(command);
        }
    }
}
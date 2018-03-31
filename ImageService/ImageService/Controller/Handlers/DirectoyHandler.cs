using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        private List<string> extentions;
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        public DirectoyHandler(IImageController controller, ILoggingService logging)
        {
            this.m_controller = controller;
            this.m_logging = logging;
            this.extentions = new List<string>();
            extentions.Add(".jpg");
            extentions.Add(".png");
            extentions.Add(".gif");
            extentions.Add(".bmp");
        }

        //start watching a Directory.
        public void StartHandleDirectory(string dirPath)
        {
            m_path = dirPath;
            m_dirWatcher = new FileSystemWatcher(m_path, "*.*");
            m_dirWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
                | NotifyFilters.DirectoryName;

            m_dirWatcher.Changed += new FileSystemEventHandler(OnChanged);
        }

        public void OnChanged(object source, FileSystemEventArgs e)
        {
            // get the file's extension 
            //string strFileExt = getFileExt(e.FullPath);
            // filter file types 

            string extension = Path.GetExtension(e.Name);
            if (extentions.Contains(extension.ToLower()))
            {
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    // OnCommandRecieved(this, new CommandRecievedEventArgs(int id, string[] args, e.FullPath));
                }
            }
        }

        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            //in case of closing all handlers - stop watching
            if (e.CommandID == (int)CommandEnum.CloseAllCommand)
            {
                m_dirWatcher.EnableRaisingEvents = false;
                m_dirWatcher.Changed -= new FileSystemEventHandler(OnClosed);
                m_dirWatcher.Dispose();
            }
            //else - check if command is relevant by comparing paths
            //else if (e.RequestDirPath.Equals(m_path))
            //{
            //    //call command by controller
            //    out bool res;
            //    m_controller.ExecuteCommand(e.CommandID, e.Args, res);
            //}
        }

        private static void OnClosed(object source, FileSystemEventArgs e)
        {
            // TODO what to do when directory closes
        }
    }
}
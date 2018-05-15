using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;

namespace ImageService.Controller.Handlers
{
    /// <summary>
    /// watches a directory and handles with changes in image files
    /// </summary>
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        // The Image Processing Controller
        private IImageController m_controller;
        private ILoggingService m_logging;
        // The Watcher of the Dir
        private FileSystemWatcher m_dirWatcher;
        // The Path of directory
        private string m_path;
        private List<string> extentions;
        #endregion

        /// <summary>
        /// The Event That Notifies that the DirectoryHandler is being closed
        /// </summary>
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="controller">commands executer</param>
        /// <param name="logging">Shows messages in EventLog</param>
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

        /// <summary>
        /// set the handler to start watching a Directory dirPath.
        /// </summary>
        /// <param name="dirPath">path to a directory which this handler start watching</param>
        public void StartHandleDirectory(string dirPath)
        {
            m_logging.Log(Messages.HandlerBeenAssigned(dirPath), MessageTypeEnum.INFO);
            m_path = dirPath;
            m_dirWatcher = new FileSystemWatcher(m_path, "*.*");
            m_dirWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
                | NotifyFilters.DirectoryName;
            m_dirWatcher.Changed += OnChanged;
            m_dirWatcher.Created += OnChanged;
            m_dirWatcher.Deleted += OnChanged;
            m_dirWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// event handler for the events of m_dirWatcher. gets called when
        /// something is changed in the directory.
        /// </summary>
        /// <param name="source">who called the func</param>
        /// <param name="e">arguments</param>
        public void OnChanged(object source, FileSystemEventArgs e)
        {
            string fileExtension = Path.GetExtension(e.Name);
            if (extentions.Contains(fileExtension.ToLower()))
            {
                switch(e.ChangeType)
                {
                    case WatcherChangeTypes.Created:
                        string fileName = System.IO.Path.GetFileName(e.FullPath);
                        string[] str = { fileName };
                        OnCommandRecieved(this, new CommandRecievedEventArgs(
                            (int)CommandEnum.NewFileCommand, str, m_path));
                        break;  
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// event handler for the sending commands event of the server.
        /// the function sends command's enum to the controller, to execute the command.
        /// notice: e.Args[0] = the file's name.
        ///         e.RequestDirPath  = a path to directory, not file!
        /// </summary>
        /// <param name="sender">who called the func</param>
        /// <param name="e"> information of command </param>
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            //in case of closing all handlers - stop watching
            if (e.CommandID == (int)CommandEnum.CloseHandlerCommand)
            {
                m_logging.Log(Messages.ClosingHandler(), MessageTypeEnum.INFO);
                CloseHandler();
                return;
            }

            // if this command is relevant to m_path
            if (e.RequestDirPath.Equals(m_path))
            {
                Task.Run(() => Thread(e));
            }
        }

        /// <summary>
        /// closing the handler - closing m_dirWatcher & invoking DirectoryClose event.
        /// </summary>
        public void CloseHandler()
        {
            try
            {
                m_dirWatcher.EnableRaisingEvents = false;
                m_dirWatcher.Changed -= OnChanged;
                m_dirWatcher.Created -= OnChanged;
                m_dirWatcher.Deleted -= OnChanged;
                m_dirWatcher.Dispose();
                DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(m_path,
                    Messages.ClosedHandlerSuccessfully(m_path)));
                m_logging.Log(Messages.ClosedHandlerSuccessfully(m_path), MessageTypeEnum.INFO);
            } catch (Exception e)
            {
                m_logging.Log(Messages.FailedClosingHandler(e.Message), MessageTypeEnum.FAIL);
            }
        }

        /// <summary>
        /// when executing a command in a new thread- this function is passed
        /// to the thread to run.
        /// </summary>
        /// <param name="e"> command info </param>
        public void Thread(CommandRecievedEventArgs e)
        {   
            m_logging.Log(Messages.HandlesIdSendingCommand(e.CommandID), MessageTypeEnum.INFO);
            string destFile = System.IO.Path.Combine(m_path, e.Args[0]);
            e.Args[0] = destFile;
            bool res;
            // outputMsg  Will be the New Path if res = true, and else will be the error message.
            string outputMsg = m_controller.ExecuteCommand(e.CommandID, e.Args, out res);
            if (res)
            {
                m_logging.Log(Messages.CommandRanSuccessfully(outputMsg), MessageTypeEnum.INFO);
            }
            else
            {
                m_logging.Log(Messages.FailedSendingCommand(outputMsg), MessageTypeEnum.FAIL);
            }
        }
    }
}
﻿using System;
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



        // The Event That Notifies that the DirectoryHandler is being closed
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;     

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

        // sends commands enum to controller.
        // e.Args[0] = the file's name.
        // e.RequestDirPath  = a path to directory, not file!
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            //in case of closing all handlers - stop watching
            if (e.CommandID == (int)CommandEnum.CloseCommand)
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

        public void AddFilesToDirRetrospectively()
        {
            var myFiles = Directory.GetFiles(m_path, "*.*", SearchOption.AllDirectories)
                 .Where(s => this.extentions.Contains((Path.GetExtension(s)).ToLower()));
            foreach (string file in myFiles)
            {
                string[] sfile = { Path.GetFileName(file) };
                OnCommandRecieved(this, new CommandRecievedEventArgs((int)CommandEnum.NewFileCommand, sfile, m_path));
            }
        }
    }
}
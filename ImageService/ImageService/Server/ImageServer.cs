using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System.Configuration;

namespace ImageService.Server
{
    class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        public ImageServer(IImageController controller, ILoggingService logging)
        {
            this.m_controller = controller;
            this.m_logging = logging;
            CreateDirectoryHandlers();
        }

        public void CreateDirectoryHandlers()
        {
            Console.WriteLine("in CreateDirectoryHandlers");
            string allDirectories = ConfigurationManager.AppSettings["Handler"];
            string[] paths = allDirectories.Split(';');
            foreach (string path in paths) { ListenToDirectory(path); }
        }

        public void ListenToDirectory(string path)
        {
            IDirectoryHandler handler = new DirectoyHandler(m_controller, m_logging);
            this.CommandRecieved += handler.OnCommandRecieved;
            handler.DirectoryClose += CloseHandler;
            handler.StartHandleDirectory(path);
            //handler.AddFilesToDirRetrospectively();
        }

        // in case the Service closes
        public void CloseHandlers()
        {
            foreach (EventHandler<CommandRecievedEventArgs> handler in CommandRecieved.GetInvocationList())
            {
                handler(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, null));
                CommandRecieved -= handler;
            }
        }

        // in case a directoryHandler closes
        // TODO: check its not creating bugs
        public void CloseHandler(object sender, DirectoryCloseEventArgs e)
        {
            if (sender is IDirectoryHandler)
            {
                ((IDirectoryHandler)sender).DirectoryClose -= CloseHandler;
                this.CommandRecieved += ((IDirectoryHandler)sender).OnCommandRecieved;

            }
        }

        public void SendCommand(int id, string[] args, string path)
        {
            this.CommandRecieved?.Invoke(this, new CommandRecievedEventArgs(id, args, path));
        }
    }
}
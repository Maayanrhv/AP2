using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Controller;
using ImageService.Controller.Handlers;
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
        }

        public void CreateDirectoryHandlers()
        {
            string allDirectories = ConfigurationManager.AppSettings["Handler"];
            string[] paths = allDirectories.Split(';');

            foreach (var path in paths) { ListenToDirectory(path); }
        }

        public void ListenToDirectory(string path)
        {
            IDirectoryHandler handler = new DirectoyHandler(m_controller, m_logging);
            this.CommandRecieved += handler.OnCommandRecieved;
            handler.DirectoryClose += CloseHandler;
            handler.StartHandleDirectory(path);
        }

        public void CloseHandlers()
        {
            foreach (EventHandler<CommandRecievedEventArgs> d in CommandRecieved.GetInvocationList())
            {
                d(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, null));
                CommandRecieved -= d;
            }
        }

        //send the command CloseHandler to the given handler
        public void CloseHandler(object sender, DirectoryCloseEventArgs e)
        {
            // TODO: check if casting is OK.  
            if (sender is IDirectoryHandler)
            {
                ((IDirectoryHandler)sender).OnCommandRecieved(this,
                    new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, null));
            }
        }

        public void SendCommand(int id, string[] args, string path)
        {
            this.CommandRecieved?.Invoke(this, new CommandRecievedEventArgs(id, args, path));
        }
    }
}
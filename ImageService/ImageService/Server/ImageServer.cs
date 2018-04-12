using System;
using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System.Configuration;

namespace ImageService.Server
{
    /// <summary>
    /// ImageServer creats handlers to every directory being watched, and
    /// sends commands to those handlers via event.
    /// </summary>
    class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        #endregion

        /// <summary>
        ///  constructor.
        /// </summary>
        /// <param name="controller">commands controller object.</param>
        /// <param name="logging">logger to pass messages to the log.</param>
        public ImageServer(IImageController controller, ILoggingService logging)
        {
            this.m_controller = controller;
            this.m_logging = logging;
            CreateDirectoryHandlers();
        }

        /// <summary>
        /// reads from App.config the path handlers that are specified; to which 
        /// directories the service will listen.
        /// </summary>
        public void CreateDirectoryHandlers()
        {
            string allDirectories = ConfigurationManager.AppSettings["Handler"];
            string[] paths = allDirectories.Split(';');
            foreach (string path in paths) { ListenToDirectory(path); }
        }

        /// <summary>
        /// sets a handler to the directory given in path parameter. 
        /// the event handler of the handler registers to the server's commands event.
        /// </summary>
        /// <param name="path">path to a directory</param>
        public void ListenToDirectory(string path)
        {
            IDirectoryHandler handler = new DirectoyHandler(m_controller, m_logging);
            this.CommandRecieved += handler.OnCommandRecieved;
            handler.DirectoryClose += HandlerIsBeingClosed;
            handler.StartHandleDirectory(path);
            // TODO: decide what to do with this:
            //try
            //{
            //    handler.AddFilesToDirRetrospectively();
            //}
            //catch (Exception e)
            //{
            //    m_logging.Log(Messages.ExceptionInfo(e), MessageTypeEnum.FAIL);
            //}
        }

         /// <summary>
         /// the function sends to all the handlers a closeCommand and
         /// takes them off the command event. called when the service is closing.
         /// </summary>
        public void CloseHandlers()
        {
            foreach (EventHandler<CommandRecievedEventArgs> handler in CommandRecieved.GetInvocationList())
            {
                handler(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, null, null));
                CommandRecieved -= handler;
            }
        }

        /// <summary>
        /// this is an event handler: when a handler is being closed this function
        /// will be called and remove HandlerIsBeingClosed from the handler's event, 
        /// and remove OnCommandRecieved(handler's function) from CommandRecieved 
        /// event.
        /// </summary>
        /// <param name="sender">who called the func HandlerIsBeingClosed </param>
        /// <param name="e">arguments</param>
        public void HandlerIsBeingClosed(object sender, DirectoryCloseEventArgs e)
        {
            if (sender is IDirectoryHandler)
            {
                ((IDirectoryHandler)sender).DirectoryClose -= HandlerIsBeingClosed;
                this.CommandRecieved -= ((IDirectoryHandler)sender).OnCommandRecieved;
            }
        }

        /// <summary>
        ///  sends the command given in the parameters to all the functions in the event
        ///  CommandRecieved.
        /// </summary>
        /// <param name="id">command id - from CommandEnum</param>
        /// <param name="args">arguments needed for the command:
        /// args[0] = the file's name</param>
        /// <param name="path">path to directory, without the file's name</param>
        public void SendCommand(int id, string[] args, string path)
        {
            this.CommandRecieved?.Invoke(this, new CommandRecievedEventArgs(id, args, path));
        }
    }
}
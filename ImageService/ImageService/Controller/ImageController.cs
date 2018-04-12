using System.Collections.Generic;
using ImageService.Commands;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;

namespace ImageService.Controller
{
     /// <summary>
     /// the controller holds a map of commands, and translate commandID to command object.
     /// </summary>
    class ImageController : IImageController
    {
        // The Modal Object
        private IImageServiceModal m_modal;
        private Dictionary<int, ICommand> commands;
        
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="modal">responsible for file system actions</param>
        public ImageController(IImageServiceModal modal)
        {
            // Storing the Modal Of The System
            m_modal = modal;
            commands = new Dictionary<int, ICommand>()
            {
                { (int)CommandEnum.NewFileCommand,  new NewFileCommand(this.m_modal) }
            };
        }

        /// <summary>
        /// call the relevant command.
        /// </summary>
        /// <param name="commandID">from commandEnum</param>
        /// <param name="args">args[0]- full path of image file, including file's name.</param>
        /// <param name="resultSuccesful">to be initialized: true if the command succeded, 
        /// false o.w.</param>
        /// <returns>the New Path if result = true, else- the error message.</returns>
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            return this.commands[commandID].Execute(args, out resultSuccesful);
        }
    }
}
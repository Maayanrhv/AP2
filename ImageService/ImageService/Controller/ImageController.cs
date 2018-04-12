using System.Collections.Generic;
using ImageService.Commands;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;

namespace ImageService.Controller
{
    /* the controller holds a map of commands, and translate commandID to command
     * object.
     */
    class ImageController : IImageController
    {
        // The Modal Object
        private IImageServiceModal m_modal;
        private Dictionary<int, ICommand> commands;

        /* constructor */
        public ImageController(IImageServiceModal modal)
        {
            // Storing the Modal Of The System
            m_modal = modal;
            commands = new Dictionary<int, ICommand>()
            {
                { (int)CommandEnum.NewFileCommand,  new NewFileCommand(this.m_modal) }
            };
        }

        /*
         * call the relevant command.
         * return value: the New Path if result = true,
         *              else- the error message.
         * param args[0] full path, including file name.
         */
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            return this.commands[commandID].Execute(args, out resultSuccesful);
        }
    }
}
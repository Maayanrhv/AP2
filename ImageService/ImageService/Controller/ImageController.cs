using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;

namespace ImageService.Controller
{
    class ImageController : IImageController
    {
        // The Modal Object
        private IImageServiceModal m_modal;
        private Dictionary<int, ICommand> commands;

        public ImageController(IImageServiceModal modal)
        {
            // Storing the Modal Of The System
            m_modal = modal;
            commands = new Dictionary<int, ICommand>()
            {
                { (int)CommandEnum.NewFileCommand,  new NewFileCommand(this.m_modal) }
            };
        }

        // The String Will Return the New Path if result = true, and will return the error message
        // args[0] - full path, including file name.
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            //TODO: thread this (Task)
            return this.commands[commandID].Execute(args, out resultSuccesful);
        }
    }
}
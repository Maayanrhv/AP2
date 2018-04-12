using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure.Enums;

namespace ImageService.Commands
{
    /*
     * can execute a specific command.
     */
    public interface ICommand
    {
        /* The Function That will Execute The command */
        string Execute(string[] args, out bool result);
    }
}

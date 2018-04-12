
using System;

namespace ImageService.Modal
{
    /// <summary>
    /// argumants object to an event handler in IDirectoryHandler.
    /// holds the information of a command to be executed.
    /// </summary>
    public class CommandRecievedEventArgs : EventArgs
    {
        // The Command ID
        public int CommandID { get; set; }
        public string[] Args { get; set; }
        // The Request Directory
        public string RequestDirPath { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="id">command id - from CommandEnum</param>
        /// <param name="args">arguments needed for the command:
        /// args[0] = the file's name</param>
        /// <param name="pathToDir">path to directory, without the file's name</param>
        public CommandRecievedEventArgs(int id, string[] args, string pathToDir)
        {
            CommandID = id;
            Args = args;
            RequestDirPath = pathToDir;
        }
    }
}

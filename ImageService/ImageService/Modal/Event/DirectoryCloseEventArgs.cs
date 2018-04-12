using System;

namespace ImageService.Modal
{
    /// <summary>
    /// argumants object to an event handler in IDirectoryHandler.
    /// holds the information of a directory / directory handler that is being closed.
    /// </summary>
    public class DirectoryCloseEventArgs : EventArgs
    {
        /// <summary>
        /// path of a directory
        /// </summary>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// The Message That goes to the logger
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dirPath">directory path</param>
        /// <param name="message">message about closing a directory</param>
        public DirectoryCloseEventArgs(string dirPath, string message)
        {
            // Setting the Directory Name
            DirectoryPath = dirPath;
            // Storing the String
            Message = message;
        }

    }
}

using System;
using ImageService.Modal;

namespace ImageService.Controller.Handlers
{
    /// <summary>
    /// watches a directory and handles with changes in image files
    /// </summary>
    public interface IDirectoryHandler
    {
        /// <summary>
        /// The Event That Notifies that the Directory is being closed
        /// </summary>
        event EventHandler<DirectoryCloseEventArgs> DirectoryClose;
        /// <summary>
        /// The Function Recieves the directory to Handle
        /// </summary>
        /// <param name="dirPath"> directory path to follow </param>
        void StartHandleDirectory(string dirPath);
        /// <summary>
        /// The Event that will be activated upon new Command.
        /// </summary>
        /// <param name="sender">who called the func</param>
        /// <param name="e"> information of command </param>
        void OnCommandRecieved(object sender, CommandRecievedEventArgs e);
        
        //TODO: remove.
        //void AddFilesToDirRetrospectively();
    }
}
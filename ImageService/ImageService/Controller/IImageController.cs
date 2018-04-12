
namespace ImageService.Controller
{
    /// <summary>
    /// can decifer a commandID and call the relevant command 
    /// </summary>
    public interface IImageController
    {
        /// <summary>
        /// call the relevant command.
        /// </summary>
        /// <param name="commandID"></param>
        /// <param name="args">arguments for the command</param>
        /// <param name="result">to be initialized: true if the command succeded, 
        /// false o.w.</param>
        /// <returns>the New Path if result = true, else- the error message.</returns>
        string ExecuteCommand(int commandID, string[] args, out bool result);          // Executing the Command Requet
    }
}

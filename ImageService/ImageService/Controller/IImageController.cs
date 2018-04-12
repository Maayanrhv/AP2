
namespace ImageService.Controller
{
    /// <summary>
    /// can decifer a commandID and call the relevant command 
    /// </summary>
    public interface IImageController
    {
        /*
         * call the relevant command.
         * return value: the New Path if result = true,
         *              else- the error message.
         */
        string ExecuteCommand(int commandID, string[] args, out bool result);          // Executing the Command Requet
    }
}

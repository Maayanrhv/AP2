
namespace ImageService.Commands
{
    /// <summary>
    /// can execute a specific command.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// The Function That will Execute The command 
        /// </summary>
        /// <param name="args">arguments for command</param>
        /// <param name="result">to be initialized: true if the command succeded, 
        /// false o.w.</param>
        /// <returns>a message: if the command succeded- information about what happend.
        /// else- error information</returns>
        string Execute(string[] args, out bool result);
    }
}

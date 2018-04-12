using ImageService.Modal;

namespace ImageService.Commands
{
    /// <summary>
    /// called when a new file got in a watched directory.
    /// </summary>
    public class NewFileCommand : ICommand
    {
        private IImageServiceModal m_modal;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="modal"></param>
        public NewFileCommand(IImageServiceModal modal)
        {
            // Storing the Modal
            m_modal = modal;
        }

        /// <summary>
        /// calls the ImageModal to add a new file.
        /// </summary>
        /// <param name="args">args[0] - the path to the file, including the file's name</param>
        /// <param name="result">to be initialized: true if the command succeded, 
        /// false o.w.</param>
        /// <returns>
        /// The String Will Return the New Path if result = true,
        /// else will return the error message.
        /// </returns>
        public string Execute(string[] args, out bool result)
        {
            string outcome = "";
            bool resAdd;
            bool resDel;
            outcome += this.m_modal.AddFile(args[0], out resAdd);
            outcome += this.m_modal.DeleteFile(args[0], out resDel);
            result = resAdd && resDel;
            return outcome;
        }
    }
}
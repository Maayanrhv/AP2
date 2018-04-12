using ImageService.Modal;

namespace ImageService.Commands
{
    /*
     * called when a new file got in a watched directory.
     */
    public class NewFileCommand : ICommand
    {
        private IImageServiceModal m_modal;

        /* constructor */
        public NewFileCommand(IImageServiceModal modal)
        {
            // Storing the Modal
            m_modal = modal;
        }

        /*
         * calls the ImageModal to add a new file.
         */ 
        /// <param args[0]> the path to the file, including the file's name.
        public string Execute(string[] args, out bool result)
        {
            // The String Will Return the New Path if result = true, and will return the error message
            return this.m_modal.AddFile(args[0], out result);
        }
    }
}
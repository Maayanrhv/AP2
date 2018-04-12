
namespace ImageService.Modal
{
    /// <summary>
    /// responsoble for basic actions in file system: add a new file to outputDir, ect.
    /// </summary>
    public interface IImageServiceModal
    {
        /// <summary>
        /// The Function Addes A file to the system
        /// </summary>
        /// <param name="path">The Path of the Image from the file</param>
        /// <returns>Indication if the Addition Was Successful</returns>
        string AddFile(string path, out bool result);
        /// <summary>
        /// deleting the file in the path given.
        /// </summary>
        /// <param name="path">origin path of an image file</param>
        /// <param name="result">to be initialaized: true if the file was added correctly, false o.w.</param>
        /// <returns>Return the New Path if result = true, else will return
        /// the error message</returns>
        string DeleteFile(string path, out bool result);

    }
}

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
    }
}
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ImageService.Infrastructure;

namespace ImageService.Modal
{
    /// <summary>
    /// responsoble for basic actions in file system: add a new file to outputDir, ect.
    /// </summary>
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        // The Output Folder
        private string m_OutputFolder;
        // The Size Of The Thumbnail Size          
        private int m_thumbnailSize;
        #endregion

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="OutputFolder">path of OutputFolder- where all new images go to</param>
        /// <param name="thumbnailSize">size of the thumbnailed images</param>
        public ImageServiceModal(string OutputFolder, int thumbnailSize)
        {
            this.m_OutputFolder = OutputFolder;
            this.m_thumbnailSize = thumbnailSize;
        }
        /// <summary>
        /// creats a new path (if not already exists) in OutputFolder.
        /// </summary>
        /// <param name="dt">specifies the year and month for the directories in the new path</param>
        /// <param name="thumbnailMonthPath">will be initialized</param>
        /// <returns>the month path</returns>
        private string CreateDirectoryInOutputDir(DateTime dt, out string thumbnailMonthPath)
        {
            string year = dt.Year.ToString();
            string month = dt.Month.ToString();
            string monthPath = m_OutputFolder + "\\" + year + "\\" + month;
            thumbnailMonthPath = m_OutputFolder + "\\Thumbnails" + "\\" + year + "\\" + month;
            // Determine whether the directory exists.
            if (!Directory.Exists(monthPath))
            {
                //create month folder
                CreateFolder(monthPath);
            }
            if (!Directory.Exists(thumbnailMonthPath))
            {
                //create thumbnail month folder
                CreateFolder(thumbnailMonthPath);
            }
            return monthPath;
        }
 
        /// <summary>
        /// with a given path - creats a directory.
        /// </summary>
        /// <param name="path">the path of the new directory</param>
        private void CreateFolder(string path)
        {
            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path))
                {
                    // Try to create the directory.
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        /// <summary>
        /// retrieves the datetime without loading the whole image.
        /// </summary>
        /// <param name="path">path to the image</param>
        /// <param name="dt">DateTime to be initialized</param>
        /// <returns></returns>
        private static string GetDateTakenFromImage(string path, out DateTime dt)
        {
            try
            {
                Regex r = new Regex(":");
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    //return DateTime.Parse(dateTaken);
                    dt = DateTime.Parse(dateTaken);
                }
                return "";
            }
            catch (Exception e)
            {
                dt = new DateTime();
                return Messages.CouldntFindDateTime() + Messages.ExceptionInfo(e);
            }
        }

        /// <summary>
        /// shrink the image in srcFile to the size of m_thumbnailSize and save it 
        /// at dstFile directory.
        /// </summary>
        /// <param name="srcFile">image path</param>
        /// <param name="dstFile">destination path</param>
        /// <param name="result">seccess or failure</param>
        private void AddThumbnailFile(string srcFile, string dstFile, out bool result)
        {
            Image im = Image.FromFile(srcFile);
            Size size = new Size(m_thumbnailSize, m_thumbnailSize);
            Bitmap bit = new Bitmap(im, size);
            try
            {
                bit.Save(dstFile);
            }
            catch (Exception e)
            {
                result = false;
                throw (e);
            } finally
            {
                im.Dispose();
            }
            result = true;
        }

        /// <summary>
        /// deleting the file in the path given.
        /// </summary>
        /// <param name="path">origin path of an image file</param>
        /// <param name="result">to be initialaized: true if the file was added correctly, false o.w.</param>
        /// <returns>Return the New Path if result = true, else will return
        /// the error message</returns>
        public string DeleteFile(string path, out bool result)
        {
            try
            {
                File.Delete(path);
            } catch (Exception e) { 
                result = false;
                return Messages.CouldntDeleteFile() + path  + Messages.ExceptionInfo(e);
            }
            result = true;
            return path + "deleted successfully";
            
        }

        /// <summary> 
        /// add the image at path to the known outputDir in a suitable hierarchy.
        /// </summary>
        /// <param name="path">origin path of an image file</param>
        /// <param name="result">to be initialaized: true if the file was added correctly, false o.w.</param>
        /// <returns>Return the New Path if result = true, else will return the error message</returns>
        public string AddFile(string path, out bool result)
        {
            System.Threading.Thread.Sleep(50);
            string retMsg = "";
            DateTime dt = new DateTime();
            //get file's creation time
            retMsg += GetDateTakenFromImage(path, out dt);
            //Create Directories if they don't exist yet
            string thumbnailMonthPath; //..\OutputDir\Thumbnails\Year\Month
            string monthPath;
            try
            {
                //..\OutputDir\Year\Month
                monthPath = CreateDirectoryInOutputDir(dt, out thumbnailMonthPath);
            } catch (Exception e) {
                retMsg += Messages.FailedToCreateFolder() + Messages.ExceptionInfo(e);
                result = false;
                return retMsg;
            }
            //add file to the OutputDir\Year\Month directory
            //get the file name only
            string fileName = Path.GetFileName(path);
            //use Path class to manipulate file and directory path.
            string destFile = Path.Combine(monthPath, fileName);
            string thumbnailDestFile = Path.Combine(thumbnailMonthPath, fileName);
            //add file to year\month directory
            File.Copy(path, destFile, true);
            try
            {
                //add file to thumbnail directory
                result = true;
                AddThumbnailFile(path, thumbnailDestFile, out result);
            } catch (Exception e) {
                retMsg += Messages.ExceptionInfo(e);
                result = false;
                return retMsg;
            }   
            retMsg += destFile;
            return retMsg;
        }
    }
}
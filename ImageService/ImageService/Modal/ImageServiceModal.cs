using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ImageService.Infrastructure;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size
        #endregion

        public ImageServiceModal(string OutputFolder, int thumbnailSize)
        {
            this.m_OutputFolder = OutputFolder;
            this.m_thumbnailSize = thumbnailSize;
        }

        private string CreateDirectoryInOutputDir(DateTime dt, out string thumbnailMonthPath)
        {
            string year = dt.Year.ToString();
            string month = dt.Month.ToString();
            string monthPath = m_OutputFolder + "\\" + year + "\\" + month;
            thumbnailMonthPath = m_OutputFolder + "\\Thumbnails" + "\\" + year + "\\" + month;
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
                //Console.WriteLine("failed to create folder: {0}", e.ToString());
            }
        }

        //retrieves the datetime without loading the whole image
        private static void GetDateTakenFromImage(string path, out DateTime dt)
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
            }
            catch (Exception e)
            {
                //   DateTime dt = new DateTime();
                dt = new DateTime();
                //  return dt;
                throw (e);
            }
        }

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
            }
            result = true;
        }

        // The String Will Return the New Path if result = true, and will return the error message
        public string AddFile(string path, out bool result)
        {
            string retMsg = "";
            DateTime dt = new DateTime();
            try
            {
                GetDateTakenFromImage(path, out dt); //get file's creation time
            }
            catch (Exception e)
            {
                retMsg += Messages.CouldntFindDateTime() + Messages.ExceptionInfo(e);
                //result = false;
                //return s;
            }
            //Create Directories if they don't exist yet
            string thumbnailMonthPath; //..\OutputDir\Thumbnails\Year\Month
            string monthPath;
            try
            {
                monthPath = CreateDirectoryInOutputDir(dt, out thumbnailMonthPath); //..\OutputDir\Year\Month
            }
            catch (Exception e)
            {
                retMsg += Messages.FailedToCreateFolder() + Messages.ExceptionInfo(e);
                result = false;
                return retMsg;//??????????????????
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
                AddThumbnailFile(path, thumbnailDestFile, out result);
            }
            catch (Exception e)
            {
                retMsg += Messages.ExceptionInfo(e);
                result = false;
                return retMsg;
            }
            retMsg += destFile;
            return retMsg;
        }
    }
}

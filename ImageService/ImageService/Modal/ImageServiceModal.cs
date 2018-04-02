using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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
                Console.WriteLine("failed to create folder: {0}", e.ToString());
            }
        }

        //retrieves the datetime without loading the whole image
        private static DateTime GetDateTakenFromImage(string path)
        {
            try
            {
                Regex r = new Regex(":");
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't find Date Time of the file.");
                DateTime dt = new DateTime();
                return dt;

            }
        }

        private bool AddThumbnailFile(string srcFile, string dstFile)
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
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        // The String Will Return the New Path if result = true, and will return the error message
        public string AddFile(string path, out bool result)
        {
            DateTime dt = GetDateTakenFromImage(path); //get file's creation time
            //Create Directories if they don't exist yet
            string thumbnailMonthPath; //..\OutputDir\Thumbnails\Year\Month
            string monthPath = CreateDirectoryInOutputDir(dt, out thumbnailMonthPath); //..\OutputDir\Year\Month
            //add file to the OutputDir\Year\Month directory
            //get the file name only
            string fileName = Path.GetFileName(path);
            //use Path class to manipulate file and directory path.
            string destFile = Path.Combine(monthPath, fileName);
            string thumbnailDestFile = Path.Combine(thumbnailMonthPath, fileName);
            //add file to year\month directory
            File.Copy(path, destFile, true);
            //add file to thumbnail directory
            result = AddThumbnailFile(path, thumbnailDestFile);
            return destFile;
        }
    }
}

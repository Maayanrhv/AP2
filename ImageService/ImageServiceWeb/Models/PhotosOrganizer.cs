using System;
using System.IO;
using System.Web;

namespace ImageServiceWeb.Models
{
    /// <summary>
    /// determines all photos' paths in Images dir and in outputDir, 
    /// and incharge of naming the photos in Images dir.
    /// </summary>
    public class PhotosOrganizer
    {
        #region paths in Web project only
        // the absolut path to Images directory in this web project
        private string absImagesPath { get; set; }
        // the relative path to Images directory in this web project
        private string relImagesPath { get; set; }
        // the absolut path to Images/Thumbnails directory in this web project
        private string absThumbnsPath { get; set; }
        // the relative path to Images/Thumbnails directory in this web project
        private string relThumbnsPath { get; set; }
        #endregion

        // the absolut path to OutputDir
        private string PathToOutputDir { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pathToOutputDir">path to outputDir</param>
        public PhotosOrganizer(string pathToOutputDir)
        {
            absImagesPath = HttpContext.Current.Server.MapPath("/Images");
            absThumbnsPath = absImagesPath + "/Thumbnails";
            relImagesPath = "/Images";
            relThumbnsPath = relImagesPath + "/Thumbnails";

            PathToOutputDir = pathToOutputDir;
        }

        /// <summary>
        /// deletes all photos in Images dir and it's subdirectories
        /// </summary>
        public void EmptyWebImagesDir()
        {
            DirectoryInfo dir = new DirectoryInfo(absImagesPath);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            dir = new DirectoryInfo(absThumbnsPath);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
        }

        /// <returns>absolut path to the thumbnail photo in outputDir</returns>
        public string SrcThumImPath(Models.Image photo)
        {
            return photo.SrcPath;
        }
        /// <returns>absolut path to the original photo in outputDir</returns>
        public string SrcFullImPath(Models.Image photo)
        {
            return PathToOutputDir + "\\" + photo.Year + "\\" + photo.Month + "\\" + photo.Name;
        }
        /// <returns>absolut path to the original photo in Images</returns>
        public string AbsProjFullImPath(Models.Image photo)
        {
            return absImagesPath + "\\" + photo.NameInWeb;
        }
        /// <returns>absolut path to the thumbnail photo in Images/thumbnails</returns>
        public string AbsProjThumImPath(Models.Image photo)
        {
            return absThumbnsPath + "\\" + photo.NameInWeb;
        }
        /// <returns>relative path to the original photo in Images/thumbnails</returns>
        public string RelProjFullImPath(Models.Image photo)
        {
            return relImagesPath + "\\" + photo.NameInWeb;
        }
        /// <returns>relative path to the thumbnail photo in Images/thumbnails</returns>
        public string RelProjThumImPath(Models.Image photo)
        {
            return relThumbnsPath+ "\\" + photo.NameInWeb;
        }

        /// <summary>
        /// generate an Image object based on the photo given.
        /// </summary>
        /// <param name="file">a photo file</param>
        /// <param name="year">the year that photo was taken</param>
        /// <param name="month">the month that photo was taken</param>
        /// <returns>an Image object representing the photo file given</returns>
        public Models.Image GeneratePhoto(FileInfo file, string year, string month)
        {
            string nameInWeb = GenerateUniqName(file.Name, file.Name, 1);
            string relPathInProj =  relThumbnsPath + "\\" + nameInWeb;
            Models.Image im = new Models.Image(file.Name, year, month, relPathInProj, file.FullName);
            im.NameInWeb = nameInWeb;
            return im;
        }

        /// <summary>
        /// returns a name that is uniq in Images directory's photos. If originalName is already
        /// exists, the function will add a number at the beggining of the new name.
        /// </summary>
        /// <param name="name">name for a photo in Images directory</param>
        /// <param name="originalName">name of photo in outputDir</param>
        /// <param name="tryNum">how many photos with the same name as originalName were found in Images
        /// directory</param>
        /// <returns>a new and uniq name</returns>
        private string GenerateUniqName(string name, string originalName, int tryNum)
        {
            string tryN;
            string absProjThumImPath = absThumbnsPath + "\\" + name;
            if (File.Exists(absProjThumImPath))
            {
                tryN = Convert.ToString(tryNum);
                return GenerateUniqName("(" + tryN + ")" + originalName, originalName, tryNum + 1);
            }
            return name;
        }
    }
}
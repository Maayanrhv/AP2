using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace ImageServiceWeb.Models
{
    /// <summary>
    /// manage the photos in the website.
    /// </summary>
    public class PhotosModel
    {
        /// <summary>
        /// all the displayed photos that in Imaged Directory
        /// </summary>
        public List<Models.Image> Photos { get; set; }
        /// <summary>
        /// a photo that the user chose to delete
        /// </summary>
        public Models.Image PhotoToDelete { get; set; }
        /// <summary>
        /// how many photos are there
        /// </summary>
        private int numOfPhotos;
        public int NumOfPhotos {
            get
            {
                if (PathToOutputDir != null)
                {
                    numOfPhotos = Directory.GetFiles(PathToOutputDir, "*.*", SearchOption.AllDirectories).Length / 2;
                }
                else
                {
                    numOfPhotos = -1;
                }
                return numOfPhotos;
            }
            private set
            {
                numOfPhotos = value;
            }
        }
        /// <summary>
        /// an absolut path to outputDir- the directory that all the photos are taken from.
        /// </summary>
        private string PathToOutputDir { get; set; }
        /// <summary>
        /// incharge of photos naming and paths in Images directory.
        /// </summary>
        private PhotosOrganizer po;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pathToDir">path to OutputDir- where all the photos are stored</param>
        public PhotosModel(string pathToDir)
        {
            if (pathToDir != null)
            {
                PathToOutputDir = pathToDir + "/OutputDir";
                NumOfPhotos = Directory.GetFiles(PathToOutputDir, "*.*", SearchOption.AllDirectories).Length / 2;
                po = new PhotosOrganizer(PathToOutputDir);
                po.EmptyWebImagesDir();
            }
            else
            {
                NumOfPhotos = -1;
            }
            this.Photos = new List<Models.Image>();
        }

        /// <summary>
        /// copy the full photo from OutputDir to Images and return the relative path
        /// </summary>
        /// <param name="photo">some photo that in the Images directory</param>
        /// <returns>the relative path to the full photo</returns>
        public string GetFullImagePath(Models.Image photo)
        {
            string fullImagePath = po.SrcFullImPath(photo);
            string projImagePath = po.AbsProjFullImPath(photo);
            if (File.Exists(fullImagePath) && !File.Exists(projImagePath))
                File.Copy(fullImagePath, projImagePath);
            return po.RelProjFullImPath(photo);
        }

        /// <summary>
        /// deletes a photo from the photos list, from Images dir, and from outputDir.
        /// if the deletions are leaving an empty folder - it is being deleted.
        /// </summary>
        /// <param name="photo">a photo to delete.</param>
        public void RemovePhoto(Models.Image photo)
        {
            try
            {
                if (this.Photos.Exists(p => p.SrcPath == photo.SrcPath))
                    this.Photos.Remove(this.Photos.Find(p => p.SrcPath == photo.SrcPath));
                //this.Photos.Remove(photo);
                string srcThumImPath, srcFullImPath, projFullImPath, projThumImPath;
                // remove from outputDir
                srcThumImPath = po.SrcThumImPath(photo);
                srcFullImPath = po.SrcFullImPath(photo);
                File.Delete(srcThumImPath);
                File.Delete(srcFullImPath);

                // check if dir is empty
                RemoveEmptyDirs(photo);

                // remove from project
                projThumImPath = po.AbsProjThumImPath(photo);
                projFullImPath = po.AbsProjFullImPath(photo);
                File.Delete(projThumImPath);
                if (File.Exists(projFullImPath))
                    File.Delete(projFullImPath);
            } catch(Exception) { }  
        }

        /// <summary>
        /// deletes empty directories that had the given photo before it was deleted.
        /// </summary>
        /// <param name="photo">some photo that was deleted</param>
        private void RemoveEmptyDirs(Models.Image photo)
        {
            // check if month dir is empty
            bool noFilesInIt, noDirsInIt;
            string srcDirPath = PathToOutputDir + "\\" + photo.Year + "\\" + photo.Month;
            string srcThumDirPath = PathToOutputDir + "\\Thumbnails" + "\\" + photo.Year + "\\" + photo.Month;
            noFilesInIt = !Directory.EnumerateFiles(srcThumDirPath).Any();
            noDirsInIt = !Directory.EnumerateDirectories(srcThumDirPath).Any();
            if (noFilesInIt && noDirsInIt)
            {
                Directory.Delete(srcThumDirPath, false);
                Directory.Delete(srcDirPath, false);
                // check year directory
                srcDirPath = PathToOutputDir + "\\" + photo.Year;
                srcThumDirPath = PathToOutputDir + "\\Thumbnails" + "\\" + photo.Year;
                noFilesInIt = !Directory.EnumerateFiles(srcDirPath).Any();
                noDirsInIt = !Directory.EnumerateDirectories(srcDirPath).Any();
                if (noFilesInIt && noDirsInIt)
                {
                    Directory.Delete(srcThumDirPath, false);
                    Directory.Delete(srcDirPath, false);
                }
            }
        }

        /// <summary>
        /// copy photos from outputDir to Images and add them to Photos list.
        /// </summary>
        public void LoadPhotos()
        {
            if (PathToOutputDir != null)
            {
                if (Directory.EnumerateFileSystemEntries(PathToOutputDir).Any())
                {
                    scanAll();
                }
            }  
        }

        /// <summary>
        /// going through all years in outputDir
        /// </summary>
        private void scanAll()
        {
            string thumbnailsPath = PathToOutputDir + "\\Thumbnails";
            string searchPattern = "*";
            DirectoryInfo di = new DirectoryInfo(thumbnailsPath);
            DirectoryInfo[] yearsDirectories =
                di.GetDirectories(searchPattern, SearchOption.TopDirectoryOnly);

            DirectoryInfo[] monthsDirectories;
            foreach (DirectoryInfo year in yearsDirectories)
            {
                monthsDirectories = year.GetDirectories(searchPattern, SearchOption.TopDirectoryOnly);
                GetPhotosFromDir(year.Name, monthsDirectories);
            }
        }
        /// <summary>
        /// going through all pictures in all month in some year
        /// </summary>
        /// <param name="year">a year directory</param>
        /// <param name="monthsDirectories">all the month directories in year</param>
        private void GetPhotosFromDir(string year, DirectoryInfo[] monthsDirectories)
        {
            Models.Image im;
            foreach (DirectoryInfo month in monthsDirectories)
            {
                FileInfo[] files =
                month.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    if (!this.Photos.Exists(photo => photo.SrcPath == file.FullName))
                    {
                        im = po.GeneratePhoto(file, year, month.Name);
                        this.Photos.Add(im);
                        if (!File.Exists(po.AbsProjThumImPath(im)))
                            File.Copy(po.SrcThumImPath(im), po.AbsProjThumImPath(im));
                    }
                }
            }
        }
    }

}
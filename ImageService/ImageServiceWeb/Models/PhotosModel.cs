using ImageService.Communication;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Web;
//using System.Windows.Forms;

namespace ImageServiceWeb.Models
{
    public class PhotosModel
    {
        public List<Models.Image> Photos { get; set; }
        public Models.Image PhotoToDelete { get; set; }
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
        private string PathToOutputDir { get; set; }

        private string absImagesPath;
        private string relImagesPath;
        private string absThumbnsPath;
        private string relThumbnsPath;

        // TODO: interface for argument
        public PhotosModel(WebModel webModel)
        {
            string pathToDir;
            if (webModel.IsServiceConnected && webModel.ConfigMap != null)
            {
                webModel.ConfigMap.TryGetValue("OutputDir", out pathToDir);
                PathToOutputDir = pathToDir + "/OutputDir";
                NumOfPhotos = Directory.GetFiles(PathToOutputDir, "*.*", SearchOption.AllDirectories).Length / 2;
            } else
            {
                NumOfPhotos = -1;
            }
            absImagesPath = HttpContext.Current.Server.MapPath("/Images");
            absThumbnsPath = absImagesPath + "/Thumbnails";
            relImagesPath = "/Images";
            relThumbnsPath = relImagesPath + "/Thumbnails";
            this.Photos = new List<Models.Image>();
            EmptyImagesDir();
        }

        public string GetFullImagePath(Models.Image photo)
        {
            string fullImagePath = PathToOutputDir + "\\" + photo.Year + "\\" + photo.Month
                + "\\" + photo.Name;
            string projImagePath = absImagesPath + "\\" + photo.Name;
            if (File.Exists(fullImagePath) && !File.Exists(projImagePath))
                File.Copy(fullImagePath, projImagePath);
            return relImagesPath + "\\" + photo.Name;
        }

        private void EmptyImagesDir()
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

        public void RemovePhoto(Models.Image photo)
        {
            try
            {
                this.Photos.Remove(photo);
                string srcThumImPath, srcFullImPath, projFullImPath, projThumImPath;
                // remove from outputDir
                srcThumImPath = photo.SrcPath;
                srcFullImPath = PathToOutputDir + "\\" + photo.Year + "\\" + photo.Month + "\\" + photo.Name;
                File.Delete(srcThumImPath);
                File.Delete(srcFullImPath);

                // check if dir is empty
                RemoveEmptyDirs(photo);

                // remove from project
                projThumImPath = absThumbnsPath + "\\" + photo.Name;
                projFullImPath = absImagesPath + "\\" + photo.Name;
                File.Delete(projThumImPath);
                if (File.Exists(projFullImPath))
                    File.Delete(projFullImPath);
            } catch(Exception) { }  
        }

        private void RemoveEmptyDirs(Models.Image photo)
        {
            // check if dir is empty
            string srcDirPath = PathToOutputDir + "\\" + photo.Year + "\\" + photo.Month;
            string srcThumDirPath = PathToOutputDir + "\\Thumbnails" + "\\" + photo.Year + "\\" + photo.Month;
            bool isEmpty = !Directory.EnumerateFiles(srcThumDirPath).Any();
            if (isEmpty)
            {
                Directory.Delete(srcThumDirPath, false);
                Directory.Delete(srcDirPath, false);
                if (!Directory.EnumerateFiles(PathToOutputDir + "\\" + photo.Year).Any())
                {
                    Directory.Delete(PathToOutputDir + "\\" + photo.Year, false);
                    Directory.Delete(PathToOutputDir + "\\Thumbnails" + "\\" + photo.Year, false);
                }
            }
        }

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
                getPhotosFromDir(year.Name, monthsDirectories);
            }
        }
        private void getPhotosFromDir(string year, DirectoryInfo[] monthsDirectories)
        {
            string photoSrc, photoDst, path;
            foreach (DirectoryInfo month in monthsDirectories)
            {
                FileInfo[] files =
                month.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    photoSrc = file.FullName;
                    photoDst = absThumbnsPath + "\\" + file.Name;
                    if (!File.Exists(photoDst))
                        File.Copy(photoSrc, photoDst);
                    if (!this.Photos.Exists(photo => photo.SrcPath == photoSrc)) {
                        path = relThumbnsPath + "\\" + file.Name;
                        this.Photos.Add(new Models.Image(file.Name, year, month.Name, path, photoSrc));
                    }
                }
            }
        }
    }

}
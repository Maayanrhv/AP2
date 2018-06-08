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

        private PhotosOrganizer po;

        // TODO: interface for argument
        public PhotosModel(WebModel webModel)
        {
            string pathToDir;
            if (webModel.IsServiceConnected && webModel.ConfigMap != null)
            {
                webModel.ConfigMap.TryGetValue("OutputDir", out pathToDir);
                PathToOutputDir = pathToDir + "/OutputDir";
                NumOfPhotos = Directory.GetFiles(PathToOutputDir, "*.*", SearchOption.AllDirectories).Length / 2;
                po = new PhotosOrganizer(PathToOutputDir);
                po.EmptyWebImagesDir();
            } else
            {
                NumOfPhotos = -1;
            }
            this.Photos = new List<Models.Image>();
        }

        public string GetFullImagePath(Models.Image photo)
        {
            string fullImagePath = po.SrcFullImPath(photo);
            string projImagePath = po.AbsProjFullImPath(photo);
            if (File.Exists(fullImagePath) && !File.Exists(projImagePath))
                File.Copy(fullImagePath, projImagePath);
            return po.RelProjFullImPath(photo);
        }

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
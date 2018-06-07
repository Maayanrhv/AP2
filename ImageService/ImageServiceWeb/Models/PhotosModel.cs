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
                if (PathToDir != null)
                {
                    numOfPhotos = Directory.GetFiles(PathToDir, "*.*", SearchOption.AllDirectories).Length / 2;
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
        private string PathToDir { get; set; }

        private string absImagesPath;
        private string absThumbnsPath;
        private string relThumbnsPath;

        // TODO: interface for argument
        public PhotosModel(WebModel webModel)
        {
            string pathToDir;
            if (webModel.IsServiceConnected && webModel.ConfigMap != null)
            {
                webModel.ConfigMap.TryGetValue("OutputDir", out pathToDir);
                PathToDir = pathToDir + "/OutputDir";
                NumOfPhotos = Directory.GetFiles(PathToDir, "*.*", SearchOption.AllDirectories).Length / 2;
            } else
            {
                NumOfPhotos = -1;
            }
            absImagesPath = HttpContext.Current.Server.MapPath("\\Images");
            absThumbnsPath = absImagesPath + "\\Thumbnails";
            relThumbnsPath = "~\\Images\\Thumbnails";

            this.Photos = new List<Models.Image>();
            //LoadPhotos();
        }

        public void LoadPhotos()
        {
            if (PathToDir != null)
            {
                if (Directory.EnumerateFileSystemEntries(PathToDir).Any())
                {
                    scanAll();
                }
            }  
        }

        private void scanAll()
        {
            string thumbnailsPath = PathToDir + "\\Thumbnails";
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
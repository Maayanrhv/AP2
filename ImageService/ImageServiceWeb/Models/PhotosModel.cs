using ImageService.Communication;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ImageServiceWeb.Models
{
    public class PhotosModel
    {
        public List<Models.Image> Photos { get; set; }
        public int numOfPhotos { get; private set; }
        private string PathToDir { get; set; }
        //private List<string> extentions = new List<string>() { ".jpg", ".png", ".gif", ".bmp"}


        // TODO: interface for argument
        public PhotosModel(WebModel webModel)
        {
            string pathToDir;
            if (webModel.IsServiceConnected && webModel.ConfigMap != null)
            {
                webModel.ConfigMap.TryGetValue("OutputDir", out pathToDir);
                PathToDir = pathToDir + "/OutputDir";
                numOfPhotos = Directory.GetFiles(PathToDir, "*.*", SearchOption.AllDirectories).Length / 2;
            } else
            {
                numOfPhotos = -1;
            }

            this.Photos = new List<Models.Image>();
            SetPhotos();
        }

        public void SetPhotos()
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
            string date;
            string prefix = "~\\Images\\OutputDir\\Thumbnails";
            foreach (DirectoryInfo month in monthsDirectories)
            {
                FileInfo[] files =
                month.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    if (year == "1")
                        date = "Unknown";
                    else
                        date = month.Name + "." + year;
                    string path = prefix + "\\" + year + "\\"+ month.Name + "\\"+ file.Name;
                    this.Photos.Add(new Models.Image
                        { Name = file.Name, Date = date, Path = path, FullPath = file.FullName });
                }
            }
        }
    }

}
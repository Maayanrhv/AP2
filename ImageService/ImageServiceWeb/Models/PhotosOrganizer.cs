using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageServiceWeb.Models
{
    public class PhotosOrganizer
    {
        // paths in Web proj only
        public string AbsImagesPath { get; private set; }
        public string RelImagesPath { get; private set; }
        public string AbsThumbnsPath { get; private set; }
        public string RelThumbnsPath { get; private set; }
        // path to outputDir
        private string PathToOutputDir { get; set; }


        public PhotosOrganizer(string pathToOutputDir)
        {
            AbsImagesPath = HttpContext.Current.Server.MapPath("/Images");
            AbsThumbnsPath = AbsImagesPath + "/Thumbnails";
            RelImagesPath = "/Images";
            RelThumbnsPath = RelImagesPath + "/Thumbnails";

            PathToOutputDir = pathToOutputDir;
        }

        public void EmptyWebImagesDir()
        {
            DirectoryInfo dir = new DirectoryInfo(AbsImagesPath);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            dir = new DirectoryInfo(AbsThumbnsPath);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
        }

        public string SrcThumImPath(Models.Image photo)
        {
            return photo.SrcPath;
        }
        public string SrcFullImPath(Models.Image photo)
        {
            return PathToOutputDir + "\\" + photo.Year + "\\" + photo.Month + "\\" + photo.Name;
        }
        public string AbsProjFullImPath(Models.Image photo)
        {
            return AbsImagesPath + "\\" + photo.NameInWeb;
        }
        public string AbsProjThumImPath(Models.Image photo)
        {
            return AbsThumbnsPath + "\\" + photo.NameInWeb;
        }
        public string RelProjFullImPath(Models.Image photo)
        {
            return RelImagesPath + "\\" + photo.NameInWeb;
        }
        public string RelProjThumImPath(Models.Image photo)
        {
            return RelThumbnsPath+ "\\" + photo.NameInWeb;
        }

        public Models.Image GeneratePhoto(FileInfo file, string year, string month)
        {
            string nameInWeb = GenerateUniqName(file.Name, file.Name, 1);
            string relPathInProj =  RelThumbnsPath + "\\" + nameInWeb;
            Models.Image im = new Models.Image(file.Name, year, month, relPathInProj, file.FullName);
            im.NameInWeb = nameInWeb;
            return im;
        }

        private string GenerateUniqName(string name, string originalName, int tryNum)
        {
            string tryN;
            string absProjThumImPath = AbsThumbnsPath + "\\" + name;
            if (File.Exists(absProjThumImPath))
            {
                tryN = Convert.ToString(tryNum);
                return GenerateUniqName("(" + tryN + ")" + originalName, originalName, tryNum + 1);
            }
            return name;
        }
    }
}
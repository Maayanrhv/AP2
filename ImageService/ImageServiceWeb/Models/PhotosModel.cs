﻿using ImageService.Communication;
using System.Collections.Generic;
using System.IO;


namespace ImageServiceWeb.Models
{
    public class PhotosModel
    {
        public List<Models.Image> Photos { get; set; }
        public int numOfPhotos { get; private set; }

        //public PhotosModel(string pathToDir)
        //{
        //    pathToDir = pathToDir + "/OutputDir";
        //    numOfPhotos = Directory.GetFiles(pathToDir, "*.*", SearchOption.AllDirectories).Length / 2;
        //}
        // TODO: interface for argument
        public PhotosModel(WebModel webModel)
        {
            string pathToDir;
            if (webModel.IsServiceConnected && webModel.ConfigMap != null)
            {
                webModel.ConfigMap.TryGetValue("OutputDir", out pathToDir);
                pathToDir = pathToDir + "/OutputDir";
                numOfPhotos = Directory.GetFiles(pathToDir, "*.*", SearchOption.AllDirectories).Length / 2;
            } else
            {
                numOfPhotos = -1;
            }
            this.Photos = new List<Models.Image>();
            SetPhotos();
        }

        private void SetPhotos()
        {
            this.Photos.Add(new Models.Image { Name = "מבאר אפעה לערד", Date = "2016", Path = "~/Images/k.jpg" });
            this.Photos.Add(new Models.Image { Name = "מצפה אלות", Date = "2016", Path = "~/Images/06. מצפה אלות (8).jpg" });
            this.Photos.Add(new Models.Image { Name = "יער יתיר", Date = "2016", Path = "~/Images/17. הרי נפתלי (3).jpg" });
            this.Photos.Add(new Models.Image { Name = "על האש יום העצמאות", Date = "2016", Path = "~/Images/11. על האש יום העצמאות (28).jpg" });
        }
    }
}
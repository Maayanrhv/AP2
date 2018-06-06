using ImageService.Communication;
using System.Collections.Generic;
using System.IO;


namespace ImageServiceWeb.Models
{
    public class PhotosModel
    {
        
        public int numOfPhotos { get; private set; }

        public PhotosModel(string pathToDir)
        {
            pathToDir = pathToDir + "/OutputDir";
            numOfPhotos = Directory.GetFiles(pathToDir, "*.*", SearchOption.AllDirectories).Length / 2;
        }
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
        }
    }
}
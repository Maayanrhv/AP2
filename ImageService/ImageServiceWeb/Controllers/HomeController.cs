using ImageServiceWeb.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using System.Threading;
using System.Linq;
using ImageService.Communication;

namespace ImageServiceWeb.Controllers
{
    /// <summary>
    /// responsible for the views logic
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// incharge of supplying and managing all data for the website, except photos info.
        /// </summary>
        static IWebModel webModel = new WebModel();
        /// <summary>
        /// incharge of managing photos.
        /// </summary>
        static IPhotosModel photosModel = new PhotosModel
            (webModel.ConfigMap!=null? webModel.ConfigMap["OutputDir"]:null);

        /// <summary>
        /// main page
        /// </summary>
        /// <returns>main page view</returns>
        public ActionResult ImageWeb()
        {
            // happens once if sevice is on
            if (!webModel.IsServiceConnected)
            {
                webModel.ConnectToService();
                photosModel = new PhotosModel
                    (webModel.ConfigMap != null ? webModel.ConfigMap["OutputDir"] : null);
            }
            // Connection to server status section
            if (webModel.IsServiceConnected)
                ViewBag.serviceStatus = "Yay! The Service is connected :)";
            else
            {
                ViewBag.serviceStatus = "Boo...The Service lost connection...";
            }

            // Photos amount section
            ViewBag.howManyPhotos = photosModel.NumOfPhotos;

            // Students info section
            ViewBag.firstName1 = ConfigurationManager.AppSettings["studentFirstName1"];
            ViewBag.firstName2 = ConfigurationManager.AppSettings["studentFirstName2"];
            ViewBag.lastName1 = ConfigurationManager.AppSettings["studentLastName1"];
            ViewBag.lastName2 = ConfigurationManager.AppSettings["studentLastName2"];
            ViewBag.id1 = ConfigurationManager.AppSettings["studentID1"];
            ViewBag.id2 = ConfigurationManager.AppSettings["studentID2"];

            return View();
        }

        /// <summary>
        /// displaying photos that added by the Service
        /// </summary>
        /// <returns>photos page view</returns>
        public ActionResult Photos()
        {
            photosModel.LoadPhotos();
            return View(photosModel.Photos);
        }

        /// <summary>
        /// displays Srvice's app.config data
        /// </summary>
        /// <returns>Configuration page view</returns>
        public ActionResult Config()
        {
            ViewBag.Handlers = new List<string>();
            string value;

            if (webModel.IsServiceConnected && webModel.ConfigMap != null)
            {
                webModel.ConfigMap.TryGetValue("OutputDir", out value);
                ViewBag.OutputDir = value;
                webModel.ConfigMap.TryGetValue("SourceName", out value);
                ViewBag.SourceName = value;
                webModel.ConfigMap.TryGetValue("LogName", out value);
                ViewBag.LogName = value;
                webModel.ConfigMap.TryGetValue("ThumbnailSize", out value);
                ViewBag.ThumbSize = value;
                ViewBag.Handlers = webModel.Handlers;
            }

            return View();
        }

        /// <summary>
        /// sends request to service to delete the chosen handler,
        /// and deletes the handler after service aproves deletion.
        /// </summary>
        /// <returns>Configuration page view</returns>
        public ActionResult HandlerDeletor()
        {
            Thread.Sleep(500);
            webModel.CloseHandler();
            return RedirectToAction("Config");
        }

        /// <summary>
        /// displays the handler that is going to be deleted, and
        /// wait to user's confirmation to delete it.
        /// </summary>
        /// <param name="h">a handler to delete</param>
        /// <returns>DeleteHandler page view</returns>
        public ActionResult DeleteHandler(string h)
        {
            ViewBag.HandlerToDelete = h;
            webModel.HandlerToDelete = h;
            return View();
        }
       
        /// <summary>
        /// displays the photo (in thumbnail size) that is going to be deleted, and
        /// wait to user's confirmation to delete it.
        /// </summary>
        /// <param name="srcPath"a photo to delete></param>
        /// <returns>DeletePhoto page view</returns>
        public ActionResult DeletePhoto(string srcPath)
        {
            Models.Image im = photosModel.Photos.Find(photo => photo.SrcPath == srcPath);
            if (im != null)
            {
                photosModel.PhotoToDelete = im;
                ViewBag.PhotoPathToDelete = im.Path;
            }
            return View(im);
        }

        /// <summary>
        /// displays the photo (in full size) that the user chose to view
        /// </summary>
        /// <param name="srcPath">the SrcPath property of some Image</param>
        /// <returns>ViewPhoto page view</returns>
        public ActionResult ViewPhoto(string srcPath)
        {
            Models.Image im = photosModel.Photos.Find(photo => photo.SrcPath == srcPath);
            ViewBag.PathToFullPic = "Non";
            if (im != null)
            {
                ViewBag.PathToFullPic = photosModel.GetFullImagePath(im);
            }
            
            return View(im);
        }

        /// <summary>
        /// deletes the chosen photo
        /// </summary>
        /// <returns>Photos page view</returns>
        public ActionResult PhotoDeletor()
        {
            Thread.Sleep(200);
            photosModel.RemovePhoto(photosModel.PhotoToDelete);
            return RedirectToAction("Photos");
        }

        /// <summary>
        /// display logs
        /// </summary>
        /// <returns>Logs page view</returns>
        public ActionResult Logs()
        {
            if (webModel.LogsList == null)
            {
                ViewBag.logsList = new List<Log>();
            } else
            {
                if (webModel.LogsList.Any() && webModel.LogsList[0].Content.Contains("Start"))
                    webModel.LogsList.Reverse();
                ViewBag.logsList = webModel.LogsList;
            }
            return View();
        }
    }
}
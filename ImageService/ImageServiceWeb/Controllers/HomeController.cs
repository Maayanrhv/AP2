﻿using ImageServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace ImageServiceWeb.Controllers
{
    public class HomeController : Controller
    {

        static WebModel webModel = new WebModel();
        static PhotosModel photosModel;

        public ActionResult ImageWeb()
        {
            // Connection to server status section
            if (webModel.IsServiceConnected)
                ViewBag.serviceStatus = "Yay! The Service is connected :)";
            else
                ViewBag.serviceStatus = "Boo...The Service lost connection...";

            // Photos amount section
            string value;
            if (webModel.IsServiceConnected)
            {
                try
                {
                    while(webModel.ConfigMap == null) { }
                    webModel.ConfigMap.TryGetValue("OutputDir", out value);
                    photosModel = new PhotosModel(value);
                    ViewBag.howManyPhotos = photosModel.numOfPhotos;
                } catch (Exception)
                {
                    ViewBag.howManyPhotos = -1;
                }
            }

            // Students info section
            ViewBag.firstName1 = ConfigurationManager.AppSettings["studentFirstName1"];
            ViewBag.firstName2 = ConfigurationManager.AppSettings["studentFirstName2"];
            ViewBag.lastName1 = ConfigurationManager.AppSettings["studentLastName1"];
            ViewBag.lastName2 = ConfigurationManager.AppSettings["studentLastName2"];
            ViewBag.id1 = ConfigurationManager.AppSettings["studentID1"];
            ViewBag.id2 = ConfigurationManager.AppSettings["studentID2"];

            return View();
        }

        public ActionResult Photos()
        {
            ViewBag.Message = "Your photos page.";

            return View();
        }

        public ActionResult Config()
        {
            string value;
            if (webModel.IsServiceConnected)
            {
                webModel.ConfigMap.TryGetValue("OutputDir", out value);
                ViewBag.OutputDir = value;
                webModel.ConfigMap.TryGetValue("SourceName", out value);
                ViewBag.SourceName = value;
                webModel.ConfigMap.TryGetValue("LogName", out value);
                ViewBag.LogName = value;
                webModel.ConfigMap.TryGetValue("ThumbnailSize", out value);
                ViewBag.ThumbSize = value;
            }

            return View();
        }


        public ActionResult Logs()
        {
            ViewBag.Message = "Your logs page.";
            return View();
        }
    }
}
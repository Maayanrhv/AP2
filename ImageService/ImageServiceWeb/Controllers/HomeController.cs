using ImageServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using ImageService.Communication;
using System.ComponentModel;

namespace ImageServiceWeb.Controllers
{
    public class HomeController : Controller
    {

        static WebModel model = new WebModel();

        public ActionResult ImageWeb()
        {
            ViewBag.firstName1 = ConfigurationManager.AppSettings["studentFirstName1"];
            ViewBag.firstName2 = ConfigurationManager.AppSettings["studentFirstName2"];
            ViewBag.lastName1 = ConfigurationManager.AppSettings["studentLastName1"];
            ViewBag.lastName2 = ConfigurationManager.AppSettings["studentLastName2"];
            ViewBag.id1 = ConfigurationManager.AppSettings["studentID1"];
            ViewBag.id2 = ConfigurationManager.AppSettings["studentID2"];

            if (model.IsServiceConnected)
                ViewBag.serviceStatus = "Yay! Service is connected :)";
            else
                ViewBag.serviceStatus = "Boo...Service lost connection...";

            return View();
        }

        public ActionResult Photos()
        {
            ViewBag.Message = "Your photos page.";

            return View();
        }

        public ActionResult Config()
        {
            ViewBag.Handlers = new List<string>();
            string value2;
            if (model.IsServiceConnected && model.ConfigMap != null)
            {
                model.ConfigMap.TryGetValue("OutputDir", out value2);
                ViewBag.OutputDir = value2;
                model.ConfigMap.TryGetValue("SourceName", out value2);
                ViewBag.SourceName = value2;
                model.ConfigMap.TryGetValue("LogName", out value2);
                ViewBag.LogName = value2;
                model.ConfigMap.TryGetValue("ThumbnailSize", out value2);
                ViewBag.ThumbSize = value2;
                ViewBag.Handlers = model.Handlers;
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
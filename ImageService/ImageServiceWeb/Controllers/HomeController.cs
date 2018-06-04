using ImageServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageServiceWeb.Controllers
{
    public class HomeController : Controller
    {
        static WebModel model = new WebModel();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Config()
        {
            string value;
            if (model.IsServiceConnected)
            {
                model.ConfigMap.TryGetValue("OutputDir", out value);
                ViewBag.OutputDir = value;
                model.ConfigMap.TryGetValue("SourceName", out value);
                ViewBag.SourceName = value;
                model.ConfigMap.TryGetValue("LogName", out value);
                ViewBag.LogName = value;
                model.ConfigMap.TryGetValue("ThumbnailSize", out value);
                ViewBag.ThumbSize = value;
            }


            return View();
        }

    }
}
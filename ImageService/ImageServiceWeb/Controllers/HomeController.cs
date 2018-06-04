using ImageServiceWeb.Models;
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
<<<<<<< HEAD
        static WebModel model = new WebModel();

        public ActionResult Index()
=======
        public ActionResult Index()
        {
            ViewBag.firstName1 = ConfigurationManager.AppSettings["studentFirstName1"];
            ViewBag.firstName2 = ConfigurationManager.AppSettings["studentFirstName2"];
            ViewBag.lastName1 = ConfigurationManager.AppSettings["studentLastName1"];
            ViewBag.lastName2 = ConfigurationManager.AppSettings["studentLastName2"];
            ViewBag.id1 = ConfigurationManager.AppSettings["studentID1"];
            ViewBag.id2 = ConfigurationManager.AppSettings["studentID2"];

            return View();
        }

        public ActionResult Config()
>>>>>>> 2154b1ce7a911a68053cc028ff93f9917d74e464
        {
            ViewBag.Message = "Your application Configuration page.";

            return View();
        }

<<<<<<< HEAD
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
=======
        public ActionResult Photos()
        {
            ViewBag.Message = "Your photos page.";
>>>>>>> 2154b1ce7a911a68053cc028ff93f9917d74e464

            return View();
        }

<<<<<<< HEAD
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

=======
        public ActionResult Logs()
        {
            ViewBag.Message = "Your logs page.";

            return View();
        }
>>>>>>> 2154b1ce7a911a68053cc028ff93f9917d74e464
    }
}
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
        public ActionResult Index()
        {
            // Connection to server status section
            ViewBag.isConnectedToServer = true;

            // Photos amount section
            ViewBag.howManyPhotos = 7;

            // Students info section
            ViewBag.firstName1 = ConfigurationManager.AppSettings["studentFirstName1"];
            ViewBag.firstName2 = ConfigurationManager.AppSettings["studentFirstName2"];
            ViewBag.lastName1 = ConfigurationManager.AppSettings["studentLastName1"];
            ViewBag.lastName2 = ConfigurationManager.AppSettings["studentLastName2"];
            ViewBag.id1 = ConfigurationManager.AppSettings["studentID1"];
            ViewBag.id2 = ConfigurationManager.AppSettings["studentID2"];

            return View();
        }

        public ActionResult Config()
        {
            ViewBag.Message = "Your application Configuration page.";

            return View();
        }

        public ActionResult Photos()
        {
            ViewBag.Message = "Your photos page.";

            return View();
        }

        public ActionResult Logs()
        {
            ViewBag.Message = "Your logs page.";

            return View();
        }
    }
}
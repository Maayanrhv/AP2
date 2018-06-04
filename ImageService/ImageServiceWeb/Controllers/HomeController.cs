using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageServiceWeb.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult ImageWeb()
        {
            return View();
        }
        public ActionResult Config()
        {
            return View();
        }
        public ActionResult Logs()
        {
            return View();
        }
        public ActionResult Photos()
        {
            return View();
        }




    }
}
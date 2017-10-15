using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rotativa;

namespace FIT5032_Week05.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("JournalMap");
        }
        public ActionResult Pdf()
        {
            //String view = viewName.Substring(viewName.LastIndexOf('/') + 1);
            return new Rotativa.MVC.ActionAsPdf("Index")
            {
                FileName = "TestViewAsPdf.pdf"
            };
        }
        public ActionResult About()
        {
            ViewBag.Message = "Sky website document page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
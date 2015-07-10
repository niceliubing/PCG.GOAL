using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PCG.GOAL.WebService.Security;

namespace PCG.GOAL.WebService.Controllers
{
    [MvcAuthorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
        public ActionResult Test()
        {

            return View("Index");
        }
    }
}

using System.Web.Mvc;

namespace PCG.GOAL.WebService.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            var baseUrl =  string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
            var token = DbConfig.GetInternalToken(baseUrl);
            ViewBag.Token = token;
            return View();
        }
    }
}
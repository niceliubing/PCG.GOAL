using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using PCG.GOAL.Common.DataAccess;
using PCG.GOAL.Common.Interface;
using PCG.GOAL.Common.WebModels;
using PCG.GOAL.WebService.Models;

namespace PCG.GOAL.WebService.Controllers
{
    public class AuthenticationController : Controller
    {
        IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private readonly IOAuthValidator _oAuthValidator;
        public AuthenticationController(IOAuthValidator oAuthValidator)
        {
            _oAuthValidator = oAuthValidator;
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel input)
        {
            if (ModelState.IsValid)
            {
                if (_oAuthValidator.ValidateUser(input.Username, input.Password))
                {
                    var password = Crypto.HashPassword(input.Password);
                    var user = new Credentials { Username = input.Username, Password = password, Role = "Admin" };
                    var identity = new ClaimsIdentity(new[] {
                            new Claim(ClaimTypes.Name, input.Username),
                            new Claim(ClaimTypes.Role, user.Role)
                        },
                        DefaultAuthenticationTypes.ApplicationCookie,
                        ClaimTypes.Name, ClaimTypes.Role);

                    // tell OWIN the identity provider, optional
                    // identity.AddClaim(new Claim(IdentityProvider, "Simplest Auth"));

                    Authentication.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = input.RememberMe
                    }, identity);

                    return RedirectToAction("Index", "Home");
                }
                ViewBag.LoginFailed = true;
            }

            return RedirectToAction("login");
        }

        public ActionResult Logout()
        {
            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("login");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using DataAccess.Common.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using PCG.GOAL.ExternalDataService.Model;
using PCG.GOAL.WebService.Models;

namespace PCG.GOAL.WebService.Controllers
{
    public class AuthenticationController : Controller
    {
        IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
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
            ViewBag.LoginFailed = false;
            ViewBag.IsDebug = false;
            if (ModelState.IsValid)
            {
                Credentials user;
                /*
                ViewBag.IsDebug = true;
                if (input.Username == "admin" && input.Password == "admin")
                {
                    var password = Crypto.HashPassword("admin");
                    user = new Credentials { Username = "admin", Password = password, Role = "Admin" };
                }
                else

                {
                    user = _apiUsers.Find(x => x.UserName == input.Username).FirstOrDefault();
                }
                */
                var password = Crypto.HashPassword("admin");
                user = new Credentials { Username = "admin", Password = password, Role = "Admin" };

                if (user != null && Crypto.VerifyHashedPassword(user.Password, input.Password))
                {
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

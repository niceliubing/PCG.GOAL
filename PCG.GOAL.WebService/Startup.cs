﻿using System;
using System.Net.Http;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using PCG.GOAL.WebService;
using PCG.GOAL.WebService.Security;

[assembly: OwinStartup(typeof(Startup))]

namespace PCG.GOAL.WebService
{
    public class Startup
    {

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static string PublicClientId { get; private set; }

        public void Configuration(IAppBuilder app)
        {


            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new GoalOAuthProvider(),
                //AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                RefreshTokenProvider = new GoalRefreshTokenProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(40),//TimeSpan.FromDays(1),
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Authentication/Login")

            });
            
            //app.UseNinjectMiddleware(NinjectConfig.CreateKernel).UseNinjectWebApi(WebApiConfig.WebApiConfiguration());
        }

    }
}

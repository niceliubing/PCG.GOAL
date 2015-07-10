using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;

namespace PCG.GOAL.WebService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            WebApiConfiguration(config);
        }

        public static HttpConfiguration WebApiConfiguration(HttpConfiguration config = null)
        {
            if (config == null)
            {
                config = new HttpConfiguration();
            }

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );
            return config;
        }
    }
}

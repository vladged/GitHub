using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http.Formatting;


namespace PbiMonitor_Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //
            config.Filters.Add(new AuthorizeAttribute());
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.Clear(); //because there are defaults of XML..
            config.Formatters.Add(new JsonMediaTypeFormatter());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using PbiMonitor_Web.Controllers;

namespace PbiMonitor_Web
{


    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            WindowsAzureActiveDirectoryBearerAuthenticationOptions AuthOptions = new WindowsAzureActiveDirectoryBearerAuthenticationOptions
            {
                Audience = WebConfigSettings.YourAPIResourceId,// ConfigurationManager.AppSettings["ida:Audience"],
                Tenant = WebConfigSettings.Tenant //ConfigurationManager.AppSettings["ida:Tenant"]
            };

            app.UseWindowsAzureActiveDirectoryBearerAuthentication(AuthOptions);


        }
    }
}

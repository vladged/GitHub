using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;


namespace PbiMonitor_Web.Controllers
{
    public static class WebConfigSettings
    {
        public static string AADAuthorityUri = WebConfigurationManager.AppSettings["AADAuthorityUri"];
      
        public static string SPA_ClientID = WebConfigurationManager.AppSettings["SPA_ClientID"]; 
        public static string SPA_ClientSecret = WebConfigurationManager.AppSettings["SPA_ClientSecret"];
        public static string PowerBiDataset = WebConfigurationManager.AppSettings["PowerBiDataset"];

        public static string RedirectUrl = WebConfigurationManager.AppSettings["RedirectUrl"];
        public static string authorityUrl = WebConfigurationManager.AppSettings["authorityUrl"];
        public static string PowerBiAPI = WebConfigurationManager.AppSettings["PowerBiAPI"];
      
        public static string YourAPIResourceId = WebConfigurationManager.AppSettings["ida:Audience"];
       // public static string YourPowerBiResourceId = WebConfigurationManager.AppSettings["ida:PowerBi"];
        public static string Tenant = WebConfigurationManager.AppSettings["ida:Tenant"];

        public static string ApiUrl = WebConfigurationManager.AppSettings["ApiUrl"];
    //    public static string GroupId = WebConfigurationManager.AppSettings["GroupId"];
        //public static string EmbedReportName = WebConfigurationManager.AppSettings["EmbedReportName"];
        //public static string EmbedReportId= WebConfigurationManager.AppSettings["EmbedReportId"];
        public static string TenantId = WebConfigurationManager.AppSettings["TenanntId"];

        public static string SPA_NotAdmin_ClientID = WebConfigurationManager.AppSettings["Demo_AdventureWorks_SPA_NotAdmin"];
        public static string SPA_NotAdmin_ClientSecret = WebConfigurationManager.AppSettings["Demo_AdventureWorks_SPA_NotAdmin_ClientSecret"];
        public static string GraphBaseUrl = WebConfigurationManager.AppSettings["GraphBaseUrl"];
    }
}
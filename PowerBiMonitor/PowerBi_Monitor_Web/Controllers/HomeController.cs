using System;
using System.Web;
using System.Web.UI;
using System.Collections.Specialized;
using Newtonsoft.Json;
//using PBIWebApp.Properties;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Web.Configuration;
using System.Web.Mvc;

using System.Management.Automation;
using System.Collections.ObjectModel;

namespace PbiMonitor_Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Main(int id=1)

        {
            if (Request.Params.Get("code") != null)
            {
                string code = Request.Params.Get("code");
                PbiMonitor_Web.Models.AccessTokenParam accessTokenParam = new Models.AccessTokenParam();
                accessTokenParam.authorizationCode = code;
                accessTokenParam.IfPowerBiAdmin = HttpContext.Cache["IfPowerBiAdmin"] as string;
                ViewBag.Title = "Pbi Monitor";
               
                return View("Main", accessTokenParam);

            }
            else
            {
                HttpContext.Cache["IfPowerBiAdmin"] =id.ToString();
                string clientId = "";
                if (id == 0)
                {
                    clientId = WebConfigSettings.SPA_NotAdmin_ClientID;
                }
                else
                {
                    clientId = WebConfigSettings.SPA_ClientID;
                }

                var @params = new NameValueCollection
                {
                    {"response_type", "code"},
                    //  {"client_id",  WebConfigSettings.SPA_ClientID}, //Crypto.DecryptString( WebConfigurationManager.AppSettings["ClientID"])
                    {"client_id",  clientId }, //Crypto.DecryptString( WebConfigurationManager.AppSettings["ClientID"])
                    { "resource", WebConfigSettings.PowerBiAPI},
                    { "redirect_uri", WebConfigSettings.RedirectUrl},
                    { "response_mode","form_post" }
                };

                //Create sign-in query string
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                queryString.Add(@params);


                Response.Redirect(String.Format(WebConfigSettings.AADAuthorityUri + "?{0}", queryString));
                return View("Router_Empty");
            }
        }


        public ActionResult Index()
        {
            string login_powerbi= "Login-PowerBI";
            string Login_PowerBIServiceAccount = "Login-PowerBIServiceAccount";
            string Get_PowerBIAccessToken = "Get-PowerBIAccessToken";

            using (PowerShell PowerShellInstance = PowerShell.Create())
            {
                // get-process | select-object -property name
                //PowerShell.Create("get-process").AddCommand("select-object").AddParameter("property", "name");

                //$secure = ConvertTo-SecureString "GameOfThrones" -AsPlainText -Force

                // PowerShellInstance.AddScript("get-process").AddCommand("select-object").AddParameter("property", "name");
                //Collection<PSObject> processes = PowerShellInstance.Invoke();

                 //$SecurePassword = "aaa" | ConvertTo-SecureString -AsPlainText -Force
                PowerShellInstance.AddCommand("ConvertTo-SecureString").AddParameter("String","GameOfThrones555").AddParameter("AsPlainText").AddParameter("Force");
                Collection<PSObject> secureString = PowerShellInstance.Invoke();

                PSCredential credentials = new PSCredential("vgedgaf@gap.com",(System.Security.SecureString) secureString[0].BaseObject);
                // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
                // use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.
                //PowerShellInstance.AddCommand(Login_PowerBIServiceAccount);
                //PowerShellInstance.Invoke();

                //Connect-PowerBIServiceAccount -Credential $creds

                PowerShellInstance.AddCommand("Connect-PowerBIServiceAccount").AddParameter("Credential", credentials);
                PowerShellInstance.Invoke();
                // use "AddParameter" to add a single parameter to the last command/script on the pipeline.
                //PowerShellInstance.AddParameter("param1", "parameter 1 value!");
                PowerShellInstance.AddCommand(Get_PowerBIAccessToken);
                Collection<PSObject> PSOutput = PowerShellInstance.Invoke();


            }

            return View();

        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
 
}
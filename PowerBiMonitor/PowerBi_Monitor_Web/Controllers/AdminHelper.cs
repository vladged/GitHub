using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using PbiMonitor_Web.SQL_Models;
using PbiMonitor_Web.Controllers;
using System.Net.Http.Headers;
using PbiMonitor_Web.Models;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using System.Web;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace PbiMonitor_Web
{
    public class AdminHelper
    {
        PbiMonitorEntities entites = new PbiMonitorEntities();

        public string NewReportNamePart1(string UserId)
        {
            if (UserId == null)
            {
                UserId = System.Web.HttpContext.Current.User.Identity.Name;
            }
            return "AdHoc_" + UserId.Replace("@", "__").Replace(".", "_") + "|";
        }
        public async Task<AuthenticationResult> ServicePrincipalAuthentication()
        {

            AuthenticationResult authenticationResult = null;
            var tenantSpecificURL = WebConfigSettings.authorityUrl.Replace("common", WebConfigSettings.TenantId);
            var authenticationContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(tenantSpecificURL);
            var credential = new ClientCredential(WebConfigSettings.SPA_ClientID, WebConfigSettings.SPA_ClientSecret);
            authenticationResult = await authenticationContext.AcquireTokenAsync(WebConfigSettings.PowerBiAPI, credential);

            return authenticationResult;
        }
        public async Task<AuthenticationResult> masterUserAccountAuthentication(string Username,string Password)
        {
            AuthenticationResult authenticationResult = null;
            var authenticationContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(WebConfigSettings.authorityUrl);
  
            var credential = new UserPasswordCredential(Username, Password);
            authenticationResult = authenticationContext.AcquireTokenAsync(WebConfigSettings.PowerBiAPI, WebConfigSettings.SPA_ClientID, credential).Result;
 
            return authenticationResult;
        }
        public List<PBI_Report> getAllReportsForUserShortNames(List<Report> AllReportsForUser, string UserId)
        {
            string NamePart1 = NewReportNamePart1(UserId);
            List<PBI_Report> result = new List<PBI_Report>();
            foreach (Report report in AllReportsForUser)
            {
                PBI_Report ict_Report = new PBI_Report(report);
                ict_Report.ShortReportName = report.Name.Replace(NamePart1, "");
                result.Add(ict_Report);
            }
            return result;
        }

        //public Report GetReportByReportName(string ReoportName,string SP_accessToken)
        //{
        //    List<Report> Allreports = getAllReportsForWorkspace(SP_accessToken);
        //    if(Allreports.Count>0)
        //    {
        //        return Allreports.Where(r => r.Name == ReoportName).First();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        //public List<Report> getAllReportsForWorkspace(string SP_accessToken)
        //{
        //   //AuthenticationResult SP_authResult = ServicePrincipalAuthentication();
        //    using (PowerBIClient client = GetClient(SP_accessToken))
        //    {
        //        string GroupId = WebConfigSettings.GroupId;
        //        List<Report> AllReports = client.Reports.GetReportsInGroup(GroupId).Value.ToList();

        //        return AllReports;
        //    }

        //}
        //public List<Report> getAllReportsForUser(string SP_accessToken, string UserId)
        //{
        //    string NamePart1 = NewReportNamePart1(UserId);
        //    List<Report> AllReports = getAllReportsForWorkspace(SP_accessToken);
        //    List<Report> result = new List<Report>();
        //    foreach (Report report in AllReports)
        //    {
        //        if (report.Name.Contains(NamePart1))// && result.Where(rep => rep.Name == report.Name).Count() == 0)
        //        {
        //            result.Add(report);
        //        }
        //    }
        //    if (result.Count() == 0)
        //    {
        //        CloneReportRequest cloneReportRequest = new CloneReportRequest();
        //        cloneReportRequest.Name = NewReportNamePart1(UserId) + "BaseTemplate";
        //        using (PowerBIClient client = GetClient(SP_accessToken))
        //        {
        //            string GroupId = WebConfigSettings.GroupId;
        //            Report clone = client.Reports.CloneReportInGroup(GroupId, WebConfigSettings.EmbedReportId, cloneReportRequest);
        //            result.Add(clone);
        //        }
        //    }
        //    return result;

        //}
        public PowerBIClient GetClient(string PowerBiaccessToken)
        {
            var tokenCredentials = new TokenCredentials(PowerBiaccessToken, "Bearer");
            string ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];
            return new PowerBIClient(new Uri(ApiUrl), tokenCredentials);
        }

        public bool CheckIfUserBelongsToADGroup(string GroupName, HttpRequestMessage request)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = GetJwtTokenFromHeader(request);
                foreach (Claim claim in jwtToken.Claims)
                {
                    if (claim.Type == "groups")
                    {
                        string group = claim.Value;
                        if (group == GroupName)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        public JwtSecurityToken GetJwtTokenFromHeader(HttpRequestMessage request)
        {
            string Bearer_AccessToken = GetBearerAccessTokenFromHeader(request);
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken_WebApi = tokenHandler.ReadToken(Bearer_AccessToken) as JwtSecurityToken;
            return jwtToken_WebApi;

        }
        public List<Claim> GetSecurityGroupsFromHeader(HttpRequestMessage request)
        {
            JwtSecurityToken jwtToken_WebApi = GetJwtTokenFromHeader(request);
            return jwtToken_WebApi.Claims.ToList();

        }
        public string GetBearerAccessTokenFromHeader(HttpRequestMessage request)
        {
            IEnumerable<string> headerValues;
            HttpRequestMessage message = request ?? new HttpRequestMessage();
            message.Headers.TryGetValues("Authorize", out headerValues);
            string Bearer_AccessToken = message.Headers.Authorization.Parameter;
            return Bearer_AccessToken;
        }
        public string AddUserToDatabase(JwtSecurityToken jwtToken_WebApi)
        {
            try
            {
                string upn = jwtToken_WebApi.Claims.FirstOrDefault(s => s.Type == "upn").Value;
                string name = jwtToken_WebApi.Claims.FirstOrDefault(s => s.Type == "name").Value;
                string userObjectID = jwtToken_WebApi.Claims.FirstOrDefault(s => s.Type == "oid").Value;

                List<t_Security_Users> users = (from rows in entites.t_Security_Users where rows.UserPrincipalName == upn select rows).ToList();
                if (users.Count == 0)
                {

                    t_Security_Users userToAdd = new t_Security_Users();
                    userToAdd.UserPrincipalName = upn;
                    userToAdd.DisplayName = name;
                    userToAdd.ObjectId = userObjectID;
                    userToAdd.IsAdmin = false;
                    userToAdd.AdHocEditor = false;
                    userToAdd.LastModified = DateTime.Now;
                    entites.t_Security_Users.Add(userToAdd);
                    entites.SaveChanges();
                    return "user added";
                }
                return "user exists";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public t_Security_Users GetUserFromDB(HttpRequestMessage request)
        {

            JwtSecurityToken jwtToken_WebApi = GetJwtTokenFromHeader(request);
            string upn = jwtToken_WebApi.Claims.FirstOrDefault(s => s.Type == "upn").Value;
            List<t_Security_Users> users = (from rows in entites.t_Security_Users where rows.UserPrincipalName == upn select rows).ToList();
            if (users.Count > 0)
            {
                return users.First();
            }
            return null;
        }
        //public async Task<AuthenticationResult> ServicePrincipalAuthentication()
        //{

        //    AuthenticationResult authenticationResult = null;
        //    var tenantSpecificURL = WebConfigSettings.authorityUrl.Replace("common", WebConfigSettings.TenantId);
        //    var authenticationContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(tenantSpecificURL);
        //    var credential = new ClientCredential(WebConfigSettings.SPA_ClientID, WebConfigSettings.SPA_ClientSecret);
        //    authenticationResult = await authenticationContext.AcquireTokenAsync(WebConfigSettings.PowerBiAPI, credential);

        //    return authenticationResult;
        //}
    }

}
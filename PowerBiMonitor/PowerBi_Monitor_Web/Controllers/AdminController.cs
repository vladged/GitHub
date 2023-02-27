using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using PbiMonitor_Web.SQL_Models;
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


namespace PbiMonitor_Web.Controllers
{
    [Authorize]
    public class AdminController : ApiController
    {

        PbiMonitorEntities entites = new PbiMonitorEntities();
        AdminHelper adminHelper = new AdminHelper();

        [HttpPost]
        public List<t_Security_Users> GetAllUsersFromDB()
        {
            t_Security_Users user = GetUserFromDB();
            if ((bool)user.IsAdmin)
            {
                List<t_Security_Users> users = (from rows in entites.t_Security_Users select rows).ToList();
                return users;
            }
            else
            {
                return null;
            }

        }
        
   
        [HttpPost]
        public t_Security_Users GetUserFromDB()
        {
            return adminHelper.GetUserFromDB(Request);

        }
  
        [HttpPost]
        public string MakeUserAdmin([FromBody]t_Security_Users user)
        {
            try
            {
               t_Security_Users DBuser = (from rows in entites.t_Security_Users where rows.UserPrincipalName == user.UserPrincipalName select rows).ToList().First();
                DBuser.IsAdmin = user.IsAdmin;
                entites.SaveChanges();
                Log.LogEvent("MakeUserAdmin", user.UserPrincipalName + " " + user.IsAdmin, 0, adminHelper.GetJwtTokenFromHeader(Request));
                return "User " + user.UserPrincipalName + " IsAdmin status switched to "+user.IsAdmin;
            }
            catch(Exception ex)
            {
                Log.LogError("MakeUserAdHocEditor", ex.Message, user.UserPrincipalName + " " + user.IsAdmin, adminHelper.GetJwtTokenFromHeader(Request));
                return ex.Message;
            }


        }
        [HttpPost]
        public string MakeUserAdHocEditor([FromBody]t_Security_Users user)
        {
            try
            {
                t_Security_Users DBuser = (from rows in entites.t_Security_Users where rows.UserPrincipalName == user.UserPrincipalName select rows).ToList().First();
                DBuser.AdHocEditor = user.AdHocEditor;
                entites.SaveChanges();
                Log.LogEvent("MakeUserAdHocEditor", user.UserPrincipalName + " " + user.AdHocEditor, 0, adminHelper.GetJwtTokenFromHeader(Request));
                return "User " + user.UserPrincipalName + " AdHocEditor status switched to " + user.AdHocEditor;
            }
            catch (Exception ex)
            {
                Log.LogError("MakeUserAdHocEditor", ex.Message, user.UserPrincipalName + " " + user.AdHocEditor, adminHelper.GetJwtTokenFromHeader(Request));
                return ex.Message;
            }
        }

   
   
    }
}

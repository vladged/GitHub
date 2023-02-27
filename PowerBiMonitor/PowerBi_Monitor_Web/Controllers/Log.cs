using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PbiMonitor_Web.SQL_Models;
using System.IdentityModel.Tokens;

namespace PbiMonitor_Web.Controllers
{
    public static class Log
    {
    
        public static void LogEvent(string Action, string Info = "", int NumbOfRec = 0, JwtSecurityToken jwtToken_WebApi = null,string message= null)
        {
           PbiMonitorEntities entites = new PbiMonitorEntities();
            try
            {
                t_Security_Log logRecord = new t_Security_Log();
                logRecord.Action = Action;
                 logRecord.InsertTimestamp = DateTime.Now;
                if (Info.Length > 99) { logRecord.Info = Info.Substring(0, 99); } else { logRecord.Info = Info; };
                logRecord.RecordsExported = NumbOfRec;
                logRecord.User = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                logRecord.Message = message;

                if (jwtToken_WebApi!=null)
                {
                    string upn = jwtToken_WebApi.Claims.FirstOrDefault(s => s.Type == "upn").Value;
                    string name = jwtToken_WebApi.Claims.FirstOrDefault(s => s.Type == "name").Value;
                    logRecord.UserNameWeb = upn;
                    logRecord.User = name; 
                }
                else
                {
                   logRecord.UserNameWeb = System.Web.HttpContext.Current.User.Identity.Name;
                }
                entites.t_Security_Log.Add(logRecord);
                entites.SaveChanges();
            }
            catch (Exception ex)
            {
                string Errmessage = ex.Message;
            }

        }
        public static void LogError(string Action, string ErrMessage ,string Info="", JwtSecurityToken jwtToken_WebApi = null)
        {
            PbiMonitorEntities entites = new PbiMonitorEntities();
            try
            {
                t_Security_Log logRecord = new t_Security_Log();
                logRecord.Action = Action;
                logRecord.InsertTimestamp = DateTime.Now;
                logRecord.Message = ErrMessage;
                logRecord.User = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
                if (Info.Length > 99) { logRecord.Info = Info.Substring(0, 99); } else { logRecord.Info = Info; };
                if (jwtToken_WebApi != null)
                {
                    string upn = jwtToken_WebApi.Claims.FirstOrDefault(s => s.Type == "upn").Value;
                    string name = jwtToken_WebApi.Claims.FirstOrDefault(s => s.Type == "name").Value;
                    logRecord.UserNameWeb = upn;
                    logRecord.User = name;
                }
                else
                {
                    logRecord.UserNameWeb = System.Web.HttpContext.Current.User.Identity.Name;
                }
                entites.t_Security_Log.Add(logRecord);
                entites.SaveChanges();
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }

        }

  

    
    }
}
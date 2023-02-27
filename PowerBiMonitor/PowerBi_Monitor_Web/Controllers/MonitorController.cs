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
//using System.IdentityModel.Tokens;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
//Groups
//GET https://api.powerbi.com/v1.0/myorg/admin/groups?$filter=state eq 'Deleted'&$top=100
//GET https://api.powerbi.com/v1.0/myorg/admin/groups?$expand=dashboards&$top=100
//GET https://api.powerbi.com/v1.0/myorg/admin/groups?$expand=datasets&$top=100
//GET https://api.powerbi.com/v1.0/myorg/admin/groups?$expand=reports&$top=100
//GET https://api.powerbi.com/v1.0/myorg/admin/groups?$expand=users&$top=100
//   "https://api.powerbi.com/v1.0/myorg/admin/groups?$expand=Reports"

//Dashboards
//GET https://api.powerbi.com/v1.0/myorg/admin/groups/f089354e-8366-4e18-aea3-4cb4a3a50b48/dashboards
//


namespace PbiMonitor_Web.Controllers
{
    [Authorize]
    public class MonitorController : ApiController
    {
        AdminHelper adminHelper = new AdminHelper();
        PbiMonitorEntities entites = new PbiMonitorEntities();
        string UsernameWeb ="Empty";
        public MonitorController()
        {
            if (System.Web.HttpContext.Current != null)
            {
                UsernameWeb = System.Web.HttpContext.Current.User.Identity.Name;
            }
        }
              //public string GetAccessToken()
              //{
              //    var credential = new UserPasswordCredential(Username, Password);

              //    // Authenticate using created credentials
              //    var authenticationContext = new AuthenticationContext(AuthorityUrl);
              //    var authenticationResult =  authenticationContext.AcquireTokenAsync(ResourceUrl, ClientId, credential);

              //}
              [HttpPost]
        public string ClearAllData([FromBody]object serialisedParam)
        {
            try
            {
                entites.TruncateAll();
                Log.LogEvent("ClearAllData", "", 0, adminHelper.GetJwtTokenFromHeader(Request));
                return "All Data is deleted";
            }
            catch (Exception ex)
            {
                Log.LogError("ClearAllData", ex.Message, "", adminHelper.GetJwtTokenFromHeader(Request));
                return ex.Message;
            }

        }
        [HttpPost]
        public List<vw_WS_DS_REP> GetGridData([FromBody]object serialisedParam)
        {
            List<vw_WS_DS_REP> GridData = (from rows in entites.vw_WS_DS_REP
                                               //    where rows.CallerPrincipalName == UsernameWeb
                                           orderby rows.workspaceName, rows.datasets_name
                                           select rows).ToList();
            return GridData;
        }
        [HttpPost]
        public string DeleteCurrentReport([FromBody]ws_ds_rep_row serialisedParam)
        {
            try
            {
                string AccessToken = serialisedParam.access_token;
                System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset + "reports/" + serialisedParam.ws_ds_rep.reports_id) as System.Net.HttpWebRequest;
                //System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset+"reports") as System.Net.HttpWebRequest;
                request.Method = "DELETE";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", "Bearer " + AccessToken);
                var response = request.GetResponse() as System.Net.HttpWebResponse;
                List<report> reportstoDelete = (from rows in entites.reports where rows.id == serialisedParam.ws_ds_rep.reports_id select rows).ToList();
                foreach (report rptodel in reportstoDelete)
                {
                    entites.reports.Remove(rptodel);
                }
                entites.SaveChanges();
                Log.LogEvent("DeleteCurrentReport", serialisedParam.ws_ds_rep.reports_name, 0, adminHelper.GetJwtTokenFromHeader(Request));
                return "Report " + serialisedParam.ws_ds_rep.reports_name + " with Id=" + serialisedParam.ws_ds_rep.reports_id + " Deleted ";
            }
            catch (Exception ex)
            {
                Log.LogError("DeleteCurrentReport", ex.Message, serialisedParam.ws_ds_rep.reports_id, adminHelper.GetJwtTokenFromHeader(Request));
                return ex.Message;
            }

        }
        [HttpPost]
        public string DeleteCurrentWorkspace([FromBody]ws_ds_rep_row serialisedParam)
        {
            try
            {
                string AccessToken = serialisedParam.access_token;
                System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset + "Workspaces/" + serialisedParam.ws_ds_rep.workspaces_id) as System.Net.HttpWebRequest;
                //System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiWorkspace+"Workspaces") as System.Net.HttpWebRequest;
                request.Method = "DELETE";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", "Bearer " + AccessToken);
                var response = request.GetResponse() as System.Net.HttpWebResponse;
                List<workspace> WorkspacestoDelete = (from rows in entites.workspaces where rows.id == serialisedParam.ws_ds_rep.workspaces_id select rows).ToList();
                foreach (workspace rptodel in WorkspacestoDelete)
                {
                    entites.workspaces.Remove(rptodel);
                }
                entites.SaveChanges();
                Log.LogEvent("DeleteCurrentWorkspace", serialisedParam.ws_ds_rep.workspaceName, 0, adminHelper.GetJwtTokenFromHeader(Request));
                return "Workspace " + serialisedParam.ws_ds_rep.workspaceName + " with Id=" + serialisedParam.ws_ds_rep.workspaces_id + " Deleted ";
            }
            catch (Exception ex)
            {
                Log.LogError("DeleteCurrentWorkspace", ex.Message, serialisedParam.ws_ds_rep.workspaces_id, adminHelper.GetJwtTokenFromHeader(Request));
                return ex.Message;
            }

        }

        [HttpPost]
        public string DeleteCurrentDataset([FromBody]ws_ds_rep_row serialisedParam)
        {
            try
            {
                string AccessToken = serialisedParam.access_token;
                System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset + "Datasets/" + serialisedParam.ws_ds_rep.datasets_id) as System.Net.HttpWebRequest;
                //System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset+"Datasets") as System.Net.HttpWebRequest;
                request.Method = "DELETE";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", "Bearer " + AccessToken);
                var response = request.GetResponse() as System.Net.HttpWebResponse;
                List<dataset> DatasetstoDelete = (from rows in entites.datasets where rows.id == serialisedParam.ws_ds_rep.datasets_id select rows).ToList();
                foreach (dataset rptodel in DatasetstoDelete)
                {
                    entites.datasets.Remove(rptodel);
                }
                entites.SaveChanges();
                Log.LogEvent("DeleteCurrentDataset", serialisedParam.ws_ds_rep.datasets_name, 0, adminHelper.GetJwtTokenFromHeader(Request));
                return "Dataset " + serialisedParam.ws_ds_rep.datasets_name + " with Id=" + serialisedParam.ws_ds_rep.datasets_id + " Deleted ";
            }
            catch (Exception ex)
            {
                Log.LogError("DeleteCurrentDataset", ex.Message, serialisedParam.ws_ds_rep.datasets_id, adminHelper.GetJwtTokenFromHeader(Request));
                return ex.Message;
            }

        }
        [HttpPost]
        public string GetPbiData([FromBody]object serialisedParam)
        {
            try
            {
                string AccessToken = ((Newtonsoft.Json.Linq.JObject)serialisedParam).First.First.ToString();
                int iNumbOfWs = PBIMWorkspaces(AccessToken);
                Log.LogEvent("GetPbiData", "Workspaces saved", iNumbOfWs, adminHelper.GetJwtTokenFromHeader(Request));
                //int iNumbOfReports = PBIMReports(AccessToken);
                //Log.LogEvent("GetPbiData", "Reports saved", iNumbOfReports, adminHelper.GetJwtTokenFromHeader(Request));
                //int iNumbOfDS = PBIMDataset(AccessToken);
                //return "Pbi Information Collected About " + iNumbOfWs.ToString()+ " Workspaces "+ iNumbOfReports.ToString()+ " Reports "+ iNumbOfDS.ToString()+" Datasets";
                return "Pbi Information Collected About " + iNumbOfWs.ToString() + " Workspaces ";
            }
            catch (Exception ex)
            {
                Log.LogError("GetPbiData", ex.Message, "", adminHelper.GetJwtTokenFromHeader(Request));
                return ex.Message;
            }
        }
        public int PBIMReports(string AccessToken, string groupid)
        {

            System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset + @"admin/groups/" + groupid + @"/reports") as System.Net.HttpWebRequest;
            //System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset+"reports") as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            try
            {
                //Get Reports response from request.GetResponse()
                using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    //Get reader from response stream

                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        //Deserialize JSON string
                        PBIReports pbiReports = JsonConvert.DeserializeObject<PBIReports>(reader.ReadToEnd());

                        foreach (PBIReport pbireport in pbiReports.value)
                        {
                            report newreport = new report();
                            newreport.CallerPrincipalName = UsernameWeb;
                            newreport.id = pbireport.id;
                            newreport.workspaceId = groupid;
                            newreport.datasetId = pbireport.datasetId;
                            newreport.name = pbireport.name;
                            newreport.webUrl = pbireport.webUrl;
                            newreport.embedUrl = pbireport.embedUrl;
                            newreport.reportType = pbireport.reportType;
                            //newreport.isOwnedByMe = pbireport.isOwnedByMe;

                            newreport.LastTimeUpdated = DateTime.Now;
                            entites.reports.Add(newreport);

                        }

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
                //  Log.LogError("PBIMWorkspaces", ex.Message, ex.InnerException.Message, adminHelper.GetJwtTokenFromHeader(Request));

            }

        }
        public int PBIMDashboards(string AccessToken)
        {

            System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset + @"admin/dashboards" ) as System.Net.HttpWebRequest;
            //System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset+"reports") as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            try
            {
                //Get Reports response from request.GetResponse()
                using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    //Get reader from response stream

                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        //Deserialize JSON string
                        PBIDashboards pbiDashboards = JsonConvert.DeserializeObject<PBIDashboards>(reader.ReadToEnd());

                        foreach (PBIDashboard pbidashboard in pbiDashboards.value)
                        {
                            dashboard newdashboard = new dashboard();
                            newdashboard.CallerPrincipalName = UsernameWeb;
                            newdashboard.id = pbidashboard.id;
                            newdashboard.displayName = pbidashboard.displayName;
                            newdashboard.embedUrl = pbidashboard.embedUrl;
                            newdashboard.isReadOnly = pbidashboard.isReadOnly;
                            //newreport.isOwnedByMe = pbireport.isOwnedByMe;

                            newdashboard.LastTimeUpdated = DateTime.Now;
                            entites.dashboards.Add(newdashboard);

                        }

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
                //  Log.LogError("PBIMWorkspaces", ex.Message, ex.InnerException.Message, adminHelper.GetJwtTokenFromHeader(Request));

            }

        }
        [HttpPost]
        public string GetScheduleHistory([FromBody]object serialisedParam)
        {
            int numbOfSchedules = 0;
            try
            {
                string AccessToken = ((Newtonsoft.Json.Linq.JObject)serialisedParam).First.First.ToString();
                List<dataset> datasets = (from rows in entites.datasets
                                          where rows.CallerPrincipalName == UsernameWeb && rows.isRefreshable == "True"
                                          select rows).ToList();

                foreach (dataset ds in datasets)
                {
                    numbOfSchedules = PBIScheduleHistory(AccessToken, ds.workspaceid, ds.id);
                }
                entites.SaveChanges();
                Log.LogEvent("GetScheduleHistory", "", numbOfSchedules, adminHelper.GetJwtTokenFromHeader(Request));
                return "Refresh histories are refreshed";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost]
        public int GetAllAdUsers([FromBody]object serialisedParam)
        {

            string AccessToken = ((Newtonsoft.Json.Linq.JObject)serialisedParam).First.First.ToString();
            string URL = @"https://graph.windows.net/"+WebConfigSettings.TenantId+ @"/users?api-version=1.6&%24top=999";
            System.Net.WebRequest request = System.Net.WebRequest.Create(URL) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            int i = 0;
            try
            {

                using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        DownloadedAdUsers allAdUsers = JsonConvert.DeserializeObject<DownloadedAdUsers>(reader.ReadToEnd());

                        foreach (Newtonsoft.Json.Linq.JObject adUser in allAdUsers.value)
                        {
                            string jsonInfo = adUser.ToString();
                            AdUser adu = new AdUser();
                            adu.CallerPrincipalName = UsernameWeb;
                            adu.LastTimeUpdated = DateTime.Now;
                            adu.JsonInfo = jsonInfo;
                            adu.RequestURL = URL;
                            entites.AdUsers.Add(adu);
                            i++;
                        }
                    }
                    entites.SaveChanges();

                }
                return i;
            }
            catch (Exception ex)
            {
                return 0;
                //  Log.LogError("PBIMWorkspaces", ex.Message, ex.InnerException.Message, adminHelper.GetJwtTokenFromHeader(Request));

            }

        }
        [HttpPost]
        public int GetUserActivitiesLast30Days([FromBody]object serialisedParam)
        {
            string AccessToken = ((Newtonsoft.Json.Linq.JObject)serialisedParam).First.First.ToString();
            int result = GetUserActivitiesLast30Days(AccessToken);
            return result;
        }
      
        public int GetUserActivitiesLast30Days(string AccessToken)
        {
            int i = 0;

            for (int DaysBack = 1; DaysBack < 30; DaysBack++)
            {
              
                string startTime = "'" + DateTime.Now.AddDays(-DaysBack).ToString("yyyy-MM-dd") + "T00:00:00" + "'";
                string startDate = DateTime.Now.AddDays(-DaysBack).ToString("yyyy-MM-dd");
                List<UserActivity> ifExists = (from rows in entites.UserActivities where rows.StartDate == startDate select rows).ToList();
                if (ifExists.Count == 0) {
                    string endTime = "'" + DateTime.Now.AddDays(-DaysBack).ToString("yyyy-MM-dd") + "T23:59:59" + "'";
                    i = i + GetUserActivitiesByURL(AccessToken, WebConfigSettings.PowerBiDataset + @"admin/activityevents?startDateTime=" + startTime + @"&endDateTime=" + endTime, startDate);
                }
            }
            return i;
        }
 
        public int GetUserActivitiesByURL(string AccessToken, string URL,string startDate,string continuationToken = "")
        {
           

            System.Net.WebRequest request = System.Net.WebRequest.Create(URL) as System.Net.HttpWebRequest;
            
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            try
            {
                int i = 0;
                //Get Reports response from request.GetResponse()
                
                using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        PBIUserActivity pbiUserActivity = JsonConvert.DeserializeObject<PBIUserActivity>(reader.ReadToEnd());
                        if (pbiUserActivity.activityEventEntities.Count() == 0)
                        {
                            UserActivity ua = new UserActivity();
                            ua.CallerPrincipalName = UsernameWeb;
                            ua.LastTimeUpdated = DateTime.Now;
                            if (pbiUserActivity.continuationToken != null)
                            {
                                ua.JsonInfo = "{"+"continuationToken"+":" + pbiUserActivity.continuationToken +"}" ;
                            }
                            ua.JsonInfo = "{}";

                            ua.RequestURL = URL;
                            ua.StartDate = startDate;
                            entites.UserActivities.Add(ua);
                        }
                        else
                        {
                            foreach (Newtonsoft.Json.Linq.JObject ev in pbiUserActivity.activityEventEntities)
                            {
                                string jsonInfo = ev.ToString();
                                UserActivity ua = new UserActivity();
                                ua.CallerPrincipalName = UsernameWeb;
                                ua.LastTimeUpdated = DateTime.Now;
                                ua.JsonInfo = jsonInfo;
                                ua.RequestURL = URL;
                                ua.StartDate = startDate;
                                entites.UserActivities.Add(ua);
                                i++;
                            }
                        }
                        entites.SaveChanges();
                        if(pbiUserActivity.continuationUri!=null && pbiUserActivity.continuationToken != "")
                        {
                            if (pbiUserActivity.continuationToken!=null)//&& !String.Equals(pbiUserActivity.continuationToken,continuationToken) && pbiUserActivity.activityEventEntities!=null && pbiUserActivity.activityEventEntities.Count()>0
                            {
                                int j = GetUserActivitiesByURL(AccessToken, WebConfigSettings.PowerBiDataset + @"admin/activityevents?continuationToken='" + pbiUserActivity.continuationToken + "'", startDate, pbiUserActivity.continuationToken);
                                //GetUserActivitiesByURL(AccessToken, pbiUserActivity.continuationUri, startDate, pbiUserActivity.continuationToken);
                                i = i + j;
                                
                            }
                        }
                    }
                 
                }
                return i;
            }
            catch (Exception ex)
            {
                return 0;
                //  Log.LogError("PBIMWorkspaces", ex.Message, ex.InnerException.Message, adminHelper.GetJwtTokenFromHeader(Request));

            }
        }



        // GET https://api.powerbi.com/v1.0/myorg/admin/activityevents
        //GET https://api.powerbi.com/v1.0/myorg/datasets/{datasetId}/refreshes?$top={$top}
        public int PBIScheduleHistory(string AccessToken, string workspaceid, string datasetId)
        {

            System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset + @"groups/" + workspaceid + @"/datasets/" + datasetId + @"/refreshes?$top=5") as System.Net.HttpWebRequest;
            //System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset+"reports") as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            try
            {
                int i = 0;
                //Get Reports response from request.GetResponse()
                using (var response = request.GetResponse() as System.Net.HttpWebResponse)
                {
                    //Get reader from response stream

                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        //Deserialize JSON string
                        PBISheduleHistories pbiScheduleHistories = JsonConvert.DeserializeObject<PBISheduleHistories>(reader.ReadToEnd());

                        foreach (PBISheduleHistory pbiSH in pbiScheduleHistories.value)
                        {
                            schedulehistory sh = new schedulehistory();
                            sh.CallerPrincipalName = UsernameWeb;
                            sh.datasetId = datasetId;
                            sh.endTime = pbiSH.endTime;
                            sh.startTime = pbiSH.startTime;
                            sh.status = pbiSH.status;
                            sh.requestId = pbiSH.requestId;
                            sh.LastTimeUpdated = DateTime.Now;
                            entites.schedulehistories.Add(sh);
                            i++;
                        }

                        return i;
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
                //  Log.LogError("PBIMWorkspaces", ex.Message, ex.InnerException.Message, adminHelper.GetJwtTokenFromHeader(Request));

            }

        }
        public int PBIMWorkspaces(string AccessToken)
        {
            //List<workspace> workspacestoDelete = (from rows in entites.workspaces where rows.CallerPrincipalName == UsernameWeb select rows).ToList();
            //foreach (workspace wktodel in workspacestoDelete)
            //{
            //    entites.workspaces.Remove(wktodel);
            //}
            //entites.SaveChanges();
            //List<user_permissions> u_ptoDelete = (from rows in entites.user_permissions where rows.CallerPrincipalName== UsernameWeb
            //                                         select rows).ToList();
            //foreach (user_permissions d in u_ptoDelete)
            //{
            //    entites.user_permissions.Remove(d);
            //}
            //entites.SaveChanges();
            //List<report> reportstoDelete = (from rows in entites.reports
            //                                where rows.CallerPrincipalName == UsernameWeb
            //                                select rows).ToList();
            //foreach (report rpttodel in reportstoDelete)
            //{
            //    entites.reports.Remove(rpttodel);
            //}
            //entites.SaveChanges();
            entites.TruncateAll();

            System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset + "admin/groups?$top=3000&$expand=users,reports,datasets,dashboards") as System.Net.HttpWebRequest;
            //System.Net.WebRequest request = System.Net.WebRequest.Create(WebConfigSettings.PowerBiDataset+"reports") as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", "Bearer " + AccessToken);

            //Get Reports response from request.GetResponse()
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                //Get reader from response stream
                try
                {
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        //Deserialize JSON string
                        PBIGroups pbigroups = JsonConvert.DeserializeObject<PBIGroups>(reader.ReadToEnd());

                        foreach (PBIGroup pbigroup in pbigroups.value)
                        {

                            workspace newworkspace = new workspace();
                            newworkspace.CallerPrincipalName = UsernameWeb;
                            newworkspace.id = pbigroup.id;
                            newworkspace.IsOnDedicatedCapacity = pbigroup.isOnDedicatedCapacity;
                            newworkspace.Description = pbigroup.description;
                            newworkspace.name = pbigroup.name;
                            newworkspace.state = pbigroup.state;
                            newworkspace.type = pbigroup.type;
                            newworkspace.isReadOnly = pbigroup.isReadOnly;
                            newworkspace.LastTimeUpdated = DateTime.Now;

                            if (pbigroup.type != "PersonalGroup")
                            {
                                //newworkspace.Accessible = PBIMReports(AccessToken, pbigroup.id);
                                //if (newworkspace.Accessible == 0)
                                //{
                                foreach (PBIReport pbireport in pbigroup.reports)
                                {

                                    report newreport = new report();
                                    newreport.CallerPrincipalName = UsernameWeb;
                                    newreport.id = pbireport.id;
                                    newreport.workspaceId = pbigroup.id;
                                    newreport.datasetId = pbireport.datasetId;
                                    newreport.name = pbireport.name;
                                    newreport.webUrl = pbireport.webUrl;
                                    newreport.embedUrl = pbireport.embedUrl;
                                    newreport.LastTimeUpdated = DateTime.Now;
                                    entites.reports.Add(newreport);

                                }
                                //   }

                                foreach (PBIUser user in pbigroup.users)
                                {

                                    user_permissions u_p = new user_permissions();
                                    u_p.CallerPrincipalName = UsernameWeb;
                                    u_p.workspaceid = pbigroup.id;
                                    u_p.UserType = user.groupUserAccessRight;
                                    u_p.LastTimeUpdated = DateTime.Now;
                                    if (user.emailAddress != null)
                                    {
                                        u_p.UserPrincipalName = user.emailAddress;
                                        entites.user_permissions.Add(u_p);
                                    }
                                    //else
                                    //{
                                    //    u_p.UserPrincipalName = i.ToString();
                                    //}

                                }

                                foreach (PBIDataset pbidataset in pbigroup.datasets)
                                {

                                    dataset newdataset = new dataset();
                                    newdataset.CallerPrincipalName = UsernameWeb;
                                    newdataset.id = pbidataset.id;

                                    //newdataset.workspaceId = pbireport.groupId;
                                    newdataset.workspaceid = pbigroup.id;
                                    newdataset.name = pbidataset.name;
                                    newdataset.addRowsAPIEnabled = pbidataset.addRowsAPIEnabled.ToString();
                                    newdataset.configuredBy = pbidataset.configuredBy;
                                    newdataset.isRefreshable = pbidataset.isRefreshable.ToString();
                                    newdataset.isEffectiveIdentityRequired = pbidataset.isEffectiveIdentityRequired.ToString();
                                    newdataset.LastTimeUpdated = DateTime.Now;
                                    entites.datasets.Add(newdataset);

                                }
                                foreach (PBIDashboard pbidashboard in pbigroup.dashboards)
                                {

                                    dashboard newdashboard = new dashboard();
                                    newdashboard.CallerPrincipalName = UsernameWeb;
                                    newdashboard.id = pbidashboard.id;

                                    //newdataset.workspaceId = pbireport.groupId;
                                    newdashboard.displayName = pbidashboard.displayName;
                                    newdashboard.embedUrl = pbidashboard.embedUrl;
                                    newdashboard.isReadOnly = pbidashboard.isReadOnly.ToString();
                                    newdashboard.workspaceid = pbigroup.id;
                                    newdashboard.LastTimeUpdated = DateTime.Now;
                                    entites.dashboards.Add(newdashboard);

                                }
                                entites.workspaces.Add(newworkspace);
                            }
                        }
                        entites.SaveChanges();
                        return pbigroups.value.Count();
                    }
                }
                catch (Exception ex)
                {
                    Log.LogError("PBIMWorkspaces", ex.Message, "", adminHelper.GetJwtTokenFromHeader(Request));
                    return -1;
                }
            }
 
        }
 


        public class ws_ds_rep_row
        {
            public vw_WS_DS_REP ws_ds_rep { get; set; }
            public string access_token { get; set; }
        }

        public class PBIReport
        {
            public string groupId { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string webUrl { get; set; }
            public string embedUrl { get; set; }
            public string datasetId { get; set; }
            public string reportType { get; set; }
            public string isFromPbix { get; set; }
            public string isOwnedByMe { get; set; }

        }
        public class PBIDashboards
        {
            public PBIDashboard[] value;
        }
        public class PBIDashboard
        {
            public string id { get; set; }
            public string displayName { get; set; }
            public string embedUrl { get; set; }
            public string isReadOnly { get; set; }
   
        }
        public class PBIReports
        {
            public PBIReport[] value;
        }
        //public class PBIDatasets
        //{
        //    public PBIDataset[] value { get; set; }
        //}
        public class PBIDataset
        {
            public string id { get; set; }
            public string name { get; set; }
            public bool addRowsAPIEnabled { get; set; }
            public string configuredBy { get; set; }
            public bool isRefreshable { get; set; }
            public bool isEffectiveIdentityRequired { get; set; }
            public bool isEffectiveIdentityRolesRequired { get; set; }
            public bool isOnPremGatewayRequired { get; set; }
        }

        public class PBIGroups
        {
            public PBIGroup[] value { get; set; }
        }
        public class PBIGroup
        {

            public string id { get; set; }
            public string isReadOnly { get; set; }
            public string isOnDedicatedCapacity { get; set; }
            public string description { get; set; }
            public string type { get; set; }
            public string state { get; set; }
            public string name { get; set; }
            public PBIUser[] users { get; set; }
            public PBIReport[] reports { get; set; }
            public PBIDataset[] datasets { get; set; }
            public PBIDashboard[] dashboards { get; set; }



            // public PBIDatasets datasets { get; set; }
        }
        //public class PBIUsers
        //{
        //    public PBIUser[] value { get; set; }
        //}
        public class PBIUser
        {
            public string emailAddress { get; set; }
            public string groupUserAccessRight { get; set; }

        }
        //        "value": [
        //  {
        //    "refreshType": "ViaApi",
        //    "startTime": "2017-06-13T09:25:43.153Z",
        //    "endTime": "2017-06-13T09:31:43.153Z",
        //    "status": "Completed",
        //    "requestId": "9399bb89-25d1-44f8-8576-136d7e9014b1"
        //  }
        //]
        public class PBISheduleHistory
        {
            public string refreshType { get; set; }
            public string startTime { get; set; }
            public string endTime { get; set; }
            public string status { get; set; }
            public string requestId { get; set; }

        }
        public class PBISheduleHistories
        {
            public PBISheduleHistory[] value { get; set; }
        }
        //   {
        //    "activityEventEntities": [],
        //    "continuationUri": "https://api.powerbi.com/v1.0/myorg/admin/activityevents?continuationToken='LDIwMjAtMDEtMjBUMDA6MDA6MDAsMjAyMC0wMS0yMFQyMzo1OTo1OSwxLCw%3D'",
        //    "continuationToken": "LDIwMjAtMDEtMjBUMDA6MDA6MDAsMjAyMC0wMS0yMFQyMzo1OTo1OSwxLCw%3D"
        //}

        public class PBIUserActivity
        {
            public Newtonsoft.Json.Linq.JObject[] activityEventEntities { get; set; }
            public string continuationUri { get; set; }
            public string continuationToken { get; set; }
        }
        public class DownloadedAdUsers
        {
            public Newtonsoft.Json.Linq.JObject[] value { get; set; }
 
        }
    }
}
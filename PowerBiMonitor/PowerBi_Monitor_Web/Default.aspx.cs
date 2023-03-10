using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace PbiMonitor_Web
{
    public partial class Default : System.Web.UI.Page
    {
        string baseUri = ConfigurationManager.AppSettings["PowerBiDataset"];
        protected void Page_Load(object sender, EventArgs e)
        {
            //Need an Authorization Code from Azure AD before you can get an access token to be able to call Power BI operations
            //You get the Authorization Code when you click Get Report (see below).
            //After you call AcquireAuthorizationCode(), Azure AD redirects back to this page with an Authorization Code.
            if (Request.Params.Get("code") != null)
            {
                //After you get an AccessToken, you can call Power BI API operations such as Get Report
                //Session["AccessToken"] = GetAccessToken(
                //    Request.Params.GetValues("code")[0],
                //    ConfigurationManager.AppSettings["AppSettingsClientID"],
                //    ConfigurationManager.AppSettings["ClientSecret"],
                //     ConfigurationManager.AppSettings["RedirectUrl"]);

                //Redirect again to get rid of code=
                Response.Redirect("/Default.aspx");
            }

            //After the redirect above to get rid of code=, Session["authResult"] does not equal null, which means you have an
            //Access Token. With the Acccess Token, you can call the Power BI Get Reports operation. Get Reports returns information
            //about a Report, not the actual Report visual. You get the Report visual later with some JavaScript. See postActionLoadReport()
            //in Default.aspx.
            if (Session["AccessToken"] != null)
            {
                //You need the Access Token in an HTML element so that the JavaScript can load a Report visual into an IFrame.
                //Without the Access Token, you can not access the Report visual.
                accessToken.Value = Session["AccessToken"].ToString();

                //In this sample, you get the first Report. In a production app, you would create a more robost
                //solution

                //Get first report. 
                GetReport(txtReportName.Text);
            }


        }
        protected void getReportButton_Click(object sender, EventArgs e)
        {
            //You need an Authorization Code from Azure AD so that you can get an Access Token
            //Values are hard-coded for sample purposes.
            GetAuthorizationCode();
        }


        //Get a Report. In this sample, you get the first Report.
        protected void GetReport(string reportName)
        {
            //Configure Reports request
            System.Net.WebRequest request = System.Net.WebRequest.Create(
                String.Format("{0}/Reports",
                baseUri)) as System.Net.HttpWebRequest;

            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken.Value));

            //Get Reports response from request.GetResponse()
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                //Get reader from response stream
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    //Deserialize JSON string
                    PBIReports Reports = JsonConvert.DeserializeObject<PBIReports>(reader.ReadToEnd());

                    //Sample assumes at least one Report.
                    //You could write an app that lists all Reports
                    if (Reports.value.Length > 0)
                    {
                        for (int i = 0; i < Reports.value.Length; i++)
                        {
                            //var report = Reports.value[index];
                            var report = Reports.value[i];
                            if (report.name == reportName)
                            {
                                hidEmbedUrl.Value = report.embedUrl;
                                hidReportId.Value = report.id;
                                //txtReportName.Text = report.name;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void GetAuthorizationCode()
        {
            //NOTE: Values are hard-coded for sample purposes.
            //Create a query string
            //Create a sign-in NameValueCollection for query string
            var @params = new NameValueCollection
            {
                //Azure AD will return an authorization code. 
                {"response_type", "code"},

                //Client ID is used by the application to identify themselves to the users that they are requesting permissions from. 
                //You get the client id when you register your Azure app.
                {"client_id",  ConfigurationManager.AppSettings["ClientID"]},

                //Resource uri to the Power BI resource to be authorized
                //The resource uri is hard-coded for sample purposes
                {"resource",  ConfigurationManager.AppSettings["PowerBiAPI"]},

                //After app authenticates, Azure AD will redirect back to the web app. In this sample, Azure AD redirects back
                //to Default page (Default.aspx).
                { "redirect_uri",  ConfigurationManager.AppSettings["RedirectUrl"]}
            };

            //Create sign-in query string
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add(@params);

            //Redirect to Azure AD Authority
            //  Authority Uri is an Azure resource that takes a client id and client secret to get an Access token
            //  QueryString contains 
            //      response_type of "code"
            //      client_id that identifies your app in Azure AD
            //      resource which is the Power BI API resource to be authorized
            //      redirect_uri which is the uri that Azure AD will redirect back to after it authenticates

            //Redirect to Azure AD to get an authorization code
            Response.Redirect(String.Format(ConfigurationManager.AppSettings["AADAuthorityUri"] + " ?{0}", queryString));
        }

    //    public string GetAccessToken(string authorizationCode, string clientID, string clientSecret, string redirectUri)
    //    {
    //        //Redirect uri must match the redirect_uri used when requesting Authorization code.
    //        //Note: If you use a redirect back to Default, as in this sample, you need to add a forward slash
    //        //such as http://localhost:13526/

    //        // Get auth token from auth code       
    //        TokenCache TC = new TokenCache();

    //        //Values are hard-coded for sample purposes
    //        string authority = ConfigurationManager.AppSettings["AADAuthorityUri"];
    //        AuthenticationContext AC = new AuthenticationContext(authority, TC);
    //        ClientCredential cc = new ClientCredential(clientID, clientSecret);

    //        //Set token from authentication result
    //        return AC.AcquireTokenByAuthorizationCode(
    //            authorizationCode,
    //            new Uri(redirectUri), cc).AccessToken;
    //    }


    }
    //Power BI Reports used to deserialize the Get Reports response.
    public class PBIReports
    {
        public PBIReport[] value { get; set; }
    }
    public class PBIReport
    {
        public string id { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }
        public string embedUrl { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using PowerBiMonitor_Scheduler.Properties;
using PbiMonitor_Web.Controllers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using PbiMonitor_Web;

namespace PowerBiMonitor_Scheduler
{
    public class Scheduler
    {
        public void SendEmailWIthGeneratedReport( int numbOfEvents)
        {
            try
            {
                string EmailServer = System.Configuration.ConfigurationManager.AppSettings["EmailServer"];
                string EmailPort = System.Configuration.ConfigurationManager.AppSettings["EmailPort"];
                string EmailServerUserId = System.Configuration.ConfigurationManager.AppSettings["EmailServerUserId"];
                string EmailServerUserPwd = System.Configuration.ConfigurationManager.AppSettings["EmailServerUserPwd"];
                string EnableSSL = System.Configuration.ConfigurationManager.AppSettings["EnableSSL"];
                string EmailUserId = System.Configuration.ConfigurationManager.AppSettings["EmailResepientUserId"];
                string EmailUserName = System.Configuration.ConfigurationManager.AppSettings["EmailRecepientUserName"];

                using (SmtpClient client = new SmtpClient())
                {
                    ;
                    client.Host = EmailServer;
                    client.Port =Convert.ToInt32(EmailPort);
                    if (EmailServerUserPwd != "")
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(EmailServerUserId, EmailServerUserPwd);
                    }
                    if (EnableSSL == "true")
                    {
                        client.EnableSsl = true;
                    }
                    using (MailMessage message = CreateMailMessage(EmailServerUserId, EmailUserId, numbOfEvents))
                    {
                        client.Send(message);
                    }
                    client.Dispose();
                }
          
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }
        private MailMessage CreateMailMessage(string UserFrom, string UserTo, int numbOfEvents)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(UserFrom);
            mailMessage.To.Add(UserTo);
            mailMessage.Body = @"";
            mailMessage.Subject = numbOfEvents.ToString()+ " UserActivity events collected " + DateTime.Today.ToString();
        
     
            return mailMessage;
        }
        public async  void CollectUserActivities()
        {
            AdminHelper adminHelper = new AdminHelper();
            MonitorController monitorController = new MonitorController();
            string MasterUserName = System.Configuration.ConfigurationManager.AppSettings["MasterUserName"];
            string MasterUserPwd = System.Configuration.ConfigurationManager.AppSettings["MasterUserPwd"];
            AuthenticationResult authResult =await  adminHelper.masterUserAccountAuthentication(MasterUserName, MasterUserPwd);
            string accessToken = authResult.AccessToken;
            int result=monitorController.GetUserActivitiesLast30Days(accessToken);
           // SendEmailWIthGeneratedReport(result);
        }
    }
}

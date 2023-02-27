using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IdentityModel.Tokens;
using System.Threading;
using System.Threading.Tasks;
using PbiMonitor_Web.Models;
using System.Security.Claims;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;



namespace PbiMonitor_Web.Controllers
{
    public class PowerBiInitController : ApiController
    {

        AdminHelper adminHelper = new AdminHelper();
   

        [AllowAnonymous]
        [HttpPost]
        public async Task<AccessTokenResponse> GetAccessTokenAsyn(AccessTokenParam ATparams)
        {

            TokenCache tokenCache = new TokenCache();

            //Values are hard-coded for sample purposes

            Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext AC = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(WebConfigSettings.AADAuthorityUri, tokenCache);
            // ClientCredential cc = new ClientCredential(WebConfigSettings.SPA_ClientID, WebConfigSettings.SPA_ClientSecret);
            ClientCredential cc = null;
            if (ATparams.IfPowerBiAdmin == "0")
            {
                cc = new ClientCredential(WebConfigSettings.SPA_NotAdmin_ClientID, WebConfigSettings.SPA_NotAdmin_ClientSecret);
            }
            else
            {
                cc = new ClientCredential(WebConfigSettings.SPA_ClientID, WebConfigSettings.SPA_ClientSecret);
            }
            AccessTokenResponse response = new AccessTokenResponse();
            response.ErrCode = "0";
            //Set token from authentication result
            try
            {
                AuthenticationResult PowerBiAuthresult = await AC.AcquireTokenByAuthorizationCodeAsync(
                    ATparams.authorizationCode, new Uri(WebConfigSettings.RedirectUrl), cc);


                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(PowerBiAuthresult.AccessToken) as JwtSecurityToken;
                string userObjectID = jwtToken.Claims.FirstOrDefault(s => s.Type == "oid").Value;
                AuthenticationResult WebApiAuthResult = await AC.AcquireTokenSilentAsync(WebConfigSettings.YourAPIResourceId, cc, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
                // string authToken_WebApi = WebApiAuthResult.AccessToken;
                JwtSecurityToken jwtToken_WebApi = tokenHandler.ReadToken(WebApiAuthResult.AccessToken) as JwtSecurityToken;

                AuthenticationResult GraphApiAuthResult = await AC.AcquireTokenSilentAsync(WebConfigSettings.GraphBaseUrl, cc, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
                // string authToken_WebApi = WebApiAuthResult.AccessToken;
                
                Log.LogEvent("GetAccessTokenAsyn", "", 0, jwtToken_WebApi);

                response.PowerBiExpiresOn = PowerBiAuthresult.ExpiresOn;
                response.PowerBiAccessToken = PowerBiAuthresult.AccessToken;
                response.userObjectID = userObjectID;
                response.WebApiAccessToken = WebApiAuthResult.AccessToken;
                response.GraphApiAccessToken = GraphApiAuthResult.AccessToken;
                //response.WebApiRefreshToken = WebApiAuthResult.RefreshToken;
                //check if the user is in DB and add him there if not
                adminHelper.AddUserToDatabase(jwtToken_WebApi);
                ////////////////////////////////////////
                return response;
    

            }
            catch (Exception ex)
            {
                response.ErrCode = "1";
                response.ErrMessage = "Your session is expired";// ex.Message;
                Log.LogError("GetAccessTokenAsyn", ex.Message, "Your session is expired");
                return response;
                //return ex.Message;
            }
        }


        [HttpPost]
        public async Task<AccessTokenResponse> GetAccessTokenByRefreshToken(AccessTokenParam ATparams)
        {


            TokenCache TC = new TokenCache();

            //Values are hard-coded for sample purposes

            Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext AC = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(WebConfigSettings.AADAuthorityUri, TC);
            ClientCredential cc = new ClientCredential(WebConfigSettings.SPA_ClientID, WebConfigSettings.SPA_ClientSecret);

            AccessTokenResponse response = new AccessTokenResponse();
            response.ErrCode = "0";
            //Set token from authentication result
            try
            {


                AuthenticationResult PowerBiAuthresult = await AC.AcquireTokenSilentAsync(WebConfigSettings.YourAPIResourceId, cc , new UserIdentifier(ATparams.userObjectID, UserIdentifierType.UniqueId));


                response.PowerBiExpiresOn = PowerBiAuthresult.ExpiresOn;
                response.PowerBiAccessToken = PowerBiAuthresult.AccessToken;
        
                return response;
            }
            catch (Exception ex)
            {
                response.ErrCode = "1";
                response.ErrMessage =ex.Message;
                Log.LogError("GetAccessTokenByRefreshToken", ex.Message);
                return response;
                //return ex.Message;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.PowerBI.Api.V2.Models;

namespace PbiMonitor_Web.Models
{

    public class FilterRequestObject
    {
        public string mkt_ProdDimKey1;
        public string merchandisekey;
        public int plan_units;
        public string plan_cost;
        public int year;
        public int monthnumber;
        public string monthname;
        public string mdl_ver_cd;
    }
    public class ExportToExcelRequest
    {
        public string pagename;
        public string visualname;
        public string csvdata;

    }
    public class ExportToExcel_DetailsRequest
    {
        public List<Filter> filters;

    }

    public class DownloadFileRequest
    {
        public string FileFullPath;

    }

    public class itemdetail
    {
        public identityitem[] identity;
        public string[] values;
    }
    public class identityitem
    {
        public Target target;
        public string equals;
    }
    public class SaveBookmark
    {
        public object allSlicerStates;
        public string BookmarkName;
  
    }
    public class SaveReport
    {
        public string SP_access_token;
        public string originalReportObjectId;
        public string reportObjectId;
        public string reportName;

    }
    public class PageChanged
    {
        public string PageName;
    }
    public class AccessTokenParam
    {
      //  public string AADAuthorityUri { get; set; }
     //   public string authorizationCode { get; set; }
        public string userObjectID { get; set; }
        //  public string WebApiRefreshToken { get; set; }
     //   public string ClientID { get; set; }
        public string authorizationCode { get; set; }
   //     public string RedirectUri { get; set; }
        public string IfPowerBiAdmin { get; set; }

    }
    public class AccessTokenResponse
    {
        public DateTimeOffset PowerBiExpiresOn { get; set; }
        public string PowerBiAccessToken { get; set; }
        public string WebApiAccessToken { get; set; }
        public string GraphApiAccessToken { get; set; }
        public string userObjectID { get; set; }
        // public string WebApiRefreshToken { get; set; }
        public string ErrCode { get; set; }
        public string ErrMessage { get; set; }
        //public HashSet<string> AllMyBrands;
        //public Boolean AdHoc;
    }
    public class EmbedTokenResponse
    {
        public EmbedToken EmbedToken;
        public string SP_access_token;
        public string EmbedUrl;
        public string ReportId;
        public string ErrCode;
        public string ErrMessage;
        public bool IsEffectiveIdentityRolesRequired;
        public string ReportName;
        public List<PBI_Report> ReportsForUser;
        public string NewReportNamePart1;

    }
    public class EmbedTokenRequest
    {
        public string PowerBiAccessToken;
        // public string WebApiAccessToken;
        public Report report;
        //public string ReportId;
        public string UserId;
    }

    //public class User1
    //{

    //    public object Id { get; set; }
    //    public object Name { get; set; }
    //    public object CustomerStatus { get; set; }
    //    public object Email { get; set; }
    //    public object SessionId { get; set; }
    //}
    public class PBI_Report : Report
    {
        public PBI_Report(Report baseReport)
        {
            this.DatasetId = baseReport.DatasetId;
            this.EmbedUrl = baseReport.EmbedUrl;
            this.Id = baseReport.Id;
            this.Name = baseReport.Name;
            this.WebUrl = baseReport.WebUrl;
            try
            {
                this.UserId = baseReport.Name.Split('|')[0];
                this.ShortReportName = baseReport.Name.Split('|')[1];
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
        }
        public bool isCurrent;
        public string ShortReportName;
        public string UserId;
    }
    public class SharedReportUser
    {
        public string UserId;
        public string ReportName;
        public bool isShared;

    }
    public class SharedBookmarkUser
    {
        public string BookmarkId;
        public string UserId;
        public string UserName;
        public bool isShared;

    }
    /// <summary>
    /// //////////////////Filters/////////////////////////
    /// </summary>
    public class Target
    {
        public string table { get; set; }
        public string column { get; set; }
    }

    public class Filter
    {
        public Target target { get; set; }
        public int filterType { get; set; }
        public string @operator { get; set; }
        public List<string> values { get; set; }


    }
    public class ExportToExcel_Details_FromPowerBiDataSet
    {
        public List<FilterRootObject> AllFilters;
        public string PowerBiAccessToken;
        public string PageName;
        public string VisualName;

    }
    public class FilterRootObject
    {
        public List<Filter> filters { get; set; }
        public List<Target> targets { get; set; }
        public string visualname { get; set; }
    }
    public partial class t_Security_Bookmarks1
    {
        public System.Guid BookmarkId { get; set; }
        public string User { get; set; }
        public string BookmarkName { get; set; }
        public string AllSlicers { get; set; }
      
    }
}

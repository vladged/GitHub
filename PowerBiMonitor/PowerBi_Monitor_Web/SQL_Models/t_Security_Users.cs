//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PbiMonitor_Web.SQL_Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class t_Security_Users
    {
        public string UserPrincipalName { get; set; }
        public string DisplayName { get; set; }
        public string ObjectId { get; set; }
        public string UserType { get; set; }
        public Nullable<bool> IsAdmin { get; set; }
        public Nullable<bool> AdHocEditor { get; set; }
        public Nullable<System.DateTime> LastModified { get; set; }
    }
}
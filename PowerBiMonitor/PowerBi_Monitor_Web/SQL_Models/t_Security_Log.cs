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
    
    public partial class t_Security_Log
    {
        public int PK { get; set; }
        public string Action { get; set; }
        public string User { get; set; }
        public Nullable<int> RecordsExported { get; set; }
        public Nullable<System.DateTime> InsertTimestamp { get; set; }
        public string UserNameWeb { get; set; }
        public string Info { get; set; }
        public string Message { get; set; }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleNotification.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class REQUEST_EXPENSE_ACTIVITY
    {
        public int ID_REQU_EXPE_ACTI { get; set; }
        public int ID_REQU_EXPE { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> ID_STAT_REQU_EXPE { get; set; }
        public Nullable<System.DateTime> DAT_STAR { get; set; }
        public string REASON_REJECT { get; set; }
        public Nullable<bool> SEND_MAIL { get; set; }
        public Nullable<System.DateTime> CREATED_MAIL { get; set; }
    
        public virtual REQUEST_EXPENSE REQUEST_EXPENSE { get; set; }
    }
}

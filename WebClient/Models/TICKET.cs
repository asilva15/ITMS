//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebClient.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TICKET
    {
        public TICKET()
        {
            this.DETAILS_TICKET = new HashSet<DETAILS_TICKET>();
            this.ATTACHEDs = new HashSet<ATTACHED>();
        }
    
        public int ID_TICK { get; set; }
        public Nullable<int> ID_ACCO { get; set; }
        public Nullable<int> ID_TYPE_TICK { get; set; }
        public Nullable<int> ID_CLIE { get; set; }
        public Nullable<int> ID_AFEC_END_CLIE { get; set; }
        public Nullable<int> ID_AREA { get; set; }
        public Nullable<int> ID_QUEU { get; set; }
        public Nullable<int> ID_PRIO { get; set; }
        public Nullable<int> ID_STAT { get; set; }
        public Nullable<int> ID_STAT_END { get; set; }
        public Nullable<int> ID_ASSI { get; set; }
        public Nullable<int> ID_SUBC { get; set; }
        public Nullable<int> ID_SUBS { get; set; }
        public Nullable<int> ID_SOUR { get; set; }
        public Nullable<System.DateTime> FEC_TICK { get; set; }
        public Nullable<System.DateTime> FEC_INI_TICK { get; set; }
        public Nullable<System.DateTime> REP_TICK { get; set; }
        public string COD_TICK { get; set; }
        public string SUM_TICK { get; set; }
        public Nullable<bool> REM_CTRL_TICK { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> CREATE_TICK { get; set; }
        public Nullable<System.DateTime> MODIFIED_TICK { get; set; }
        public Nullable<System.DateTime> DELETE_TICK { get; set; }
        public Nullable<System.DateTime> FOR_REP { get; set; }
        public Nullable<int> ID_TICK_PARENT { get; set; }
        public Nullable<bool> IS_PARENT { get; set; }
        public Nullable<bool> IS_ROLE_USER { get; set; }
        public Nullable<int> ID_COMP { get; set; }
        public Nullable<int> ID_COMP_END { get; set; }
        public Nullable<int> ID_DOCU_SALE { get; set; }
        public Nullable<int> ID_DETA_DOCU_SALE { get; set; }
        public Nullable<int> ID_DETA_SALE { get; set; }
        public Nullable<int> ID_CATE { get; set; }
        public Nullable<int> ID_PERS_ENTI { get; set; }
        public Nullable<int> ID_PERS_ENTI_END { get; set; }
        public Nullable<int> ID_PERS_ENTI_ASSI { get; set; }
        public Nullable<int> ID_ASSE { get; set; }
        public Nullable<int> ID_TYPE_FORM { get; set; }
        public Nullable<int> ID_LOCA { get; set; }
        public Nullable<int> MINUTES { get; set; }
        public Nullable<System.DateTime> DAT_EXPI_TICK { get; set; }
        public Nullable<int> ID_ALER_MAIL_TICK { get; set; }
        public Nullable<int> ID_PERS_ENTI_ASSI_STAR { get; set; }
        public Nullable<int> ID_STAT_SALE_OPPO { get; set; }
        public Nullable<decimal> AMM_SALE_OPPO { get; set; }
        public Nullable<bool> SEND_SURVEY { get; set; }
    
        public virtual ICollection<DETAILS_TICKET> DETAILS_TICKET { get; set; }
        public virtual PRIORITY PRIORITY { get; set; }
        public virtual SOURCE SOURCE { get; set; }
        public virtual STATUS STATUS { get; set; }
        public virtual STATUS STATUS1 { get; set; }
        public virtual TYPE_TICKET TYPE_TICKET { get; set; }
        public virtual QUEUE QUEUE { get; set; }
        public virtual ICollection<ATTACHED> ATTACHEDs { get; set; }
    }
}

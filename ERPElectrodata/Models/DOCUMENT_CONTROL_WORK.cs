//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERPElectrodata.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DOCUMENT_CONTROL_WORK
    {
        public int ID_DOCU_CONT_WORK { get; set; }
        public Nullable<int> ID_DOCU_CONT_RECE { get; set; }
        public Nullable<int> ID_DOCU_CONT { get; set; }
        public Nullable<int> ID_PERS_ENTI { get; set; }
        public Nullable<int> ID_DOCU_CONT_ACTI { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<bool> SEND_MAIL { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
    
        public virtual DOCUMENT_CONTROL_RECEIVER DOCUMENT_CONTROL_RECEIVER { get; set; }
        public virtual DOCUMENT_CONTROL_ACTION DOCUMENT_CONTROL_ACTION { get; set; }
    }
}

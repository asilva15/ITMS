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
    
    public partial class DOCUMENT_CONTROL_RECEIVER
    {
        public DOCUMENT_CONTROL_RECEIVER()
        {
            this.DOCUMENT_CONTROL_WORK = new HashSet<DOCUMENT_CONTROL_WORK>();
        }
    
        public int ID_DOCU_CONT_RECE { get; set; }
        public Nullable<int> ID_DOCU_CONT { get; set; }
        public Nullable<int> ID_PERS_ENTI { get; set; }
        public Nullable<int> ID_DOCU_CONT_STOR { get; set; }
        public Nullable<int> ID_DOCU_CONT_ROL { get; set; }
        public Nullable<int> ID_DOCU_CONT_ACTI { get; set; }
        public Nullable<int> ID_CHAR { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
    
        public virtual ICollection<DOCUMENT_CONTROL_WORK> DOCUMENT_CONTROL_WORK { get; set; }
    }
}

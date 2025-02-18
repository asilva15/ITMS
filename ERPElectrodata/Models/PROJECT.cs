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
    
    public partial class PROJECT
    {
        public PROJECT()
        {
            this.HITOes = new HashSet<HITO>();
            this.ATTACHED_PROJECT = new HashSet<ATTACHED_PROJECT>();
            this.TEAMs = new HashSet<TEAM>();
            this.OP_PROJECT = new HashSet<OP_PROJECT>();
            this.TASKs = new HashSet<TASK>();
            this.DOCUMENT_SALE = new HashSet<DOCUMENT_SALE>();
        }
    
        public int ID_PROJ { get; set; }
        public string NAM_PROJ { get; set; }
        public string DES_PROJ { get; set; }
        public string COD_PROJ { get; set; }
        public Nullable<System.DateTime> DATE_STAR { get; set; }
        public Nullable<System.DateTime> DATE_END { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public Nullable<System.DateTime> MODIFIED { get; set; }
    
        public virtual ICollection<HITO> HITOes { get; set; }
        public virtual ICollection<ATTACHED_PROJECT> ATTACHED_PROJECT { get; set; }
        public virtual ICollection<TEAM> TEAMs { get; set; }
        public virtual ICollection<OP_PROJECT> OP_PROJECT { get; set; }
        public virtual ICollection<TASK> TASKs { get; set; }
        public virtual ICollection<DOCUMENT_SALE> DOCUMENT_SALE { get; set; }
    }
}

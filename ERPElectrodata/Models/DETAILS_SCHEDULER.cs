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
    
    public partial class DETAILS_SCHEDULER
    {
        public long ID_DETA_SCHE { get; set; }
        public Nullable<int> ID_SCHE { get; set; }
        public Nullable<System.DateTime> STA_DATE { get; set; }
        public Nullable<System.DateTime> END_DATE { get; set; }
    
        public virtual SCHEDULER SCHEDULER { get; set; }
    }
}

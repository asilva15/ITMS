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
    
    public partial class CHART
    {
        public CHART()
        {
            this.RESPONSIBLE_CHART = new HashSet<RESPONSIBLE_CHART>();
            this.SCHEDULER_CHART = new HashSet<SCHEDULER_CHART>();
        }
    
        public int ID_CHAR { get; set; }
        public Nullable<int> ID_CHAR_PARE { get; set; }
        public Nullable<int> ID_NAM_CHAR { get; set; }
        public Nullable<int> ID_ACCO { get; set; }
        public Nullable<bool> VIG_CHAR { get; set; }
        public Nullable<int> ID_RECE_MAIL { get; set; }
        public Nullable<int> IdCompania { get; set; }
    
        public virtual NAME_CHART NAME_CHART { get; set; }
        public virtual ICollection<RESPONSIBLE_CHART> RESPONSIBLE_CHART { get; set; }
        public virtual ICollection<SCHEDULER_CHART> SCHEDULER_CHART { get; set; }
    }
}

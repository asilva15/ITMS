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
    
    public partial class TYPE_TICKET
    {
        public TYPE_TICKET()
        {
            this.SLAs = new HashSet<SLA>();
            this.TICKETs = new HashSet<TICKET>();
        }
    
        public int ID_TYPE_TICK { get; set; }
        public string NAM_TYPE_TICK { get; set; }
        public string DESC_TYPE_TICK { get; set; }
        public Nullable<bool> VIG_TYPE_TICK { get; set; }
        public string COL_TYPE_TICK { get; set; }
        public Nullable<bool> VIS_TYPE_TICK { get; set; }
    
        public virtual ICollection<SLA> SLAs { get; set; }
        public virtual ICollection<TICKET> TICKETs { get; set; }
    }
}

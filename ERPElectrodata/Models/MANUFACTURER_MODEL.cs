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
    
    public partial class MANUFACTURER_MODEL
    {
        public MANUFACTURER_MODEL()
        {
            this.ASSETs = new HashSet<ASSET>();
        }
    
        public int ID_MANU_MODE { get; set; }
        public Nullable<int> ID_MANU { get; set; }
        public string NAM_MANU_MODE { get; set; }
        public string DESC_MANU_MODE { get; set; }
        public Nullable<int> UserIdCreacion { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public Nullable<int> UserIdModifica { get; set; }
        public Nullable<System.DateTime> FechaModifica { get; set; }
    
        public virtual MANUFACTURER MANUFACTURER { get; set; }
        public virtual ICollection<ASSET> ASSETs { get; set; }
    }
}

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
    
    public partial class CONDITION
    {
        public CONDITION()
        {
            this.CLIENT_ASSET = new HashSet<CLIENT_ASSET>();
            this.ComponenteStockDetalles = new HashSet<ComponenteStockDetalle>();
            this.ComponenteStockCabecera = new HashSet<ComponenteStockCabecera>();
        }
    
        public int ID_COND { get; set; }
        public Nullable<int> ID_STAT_ASSE { get; set; }
        public string NAM_COND { get; set; }
        public Nullable<bool> VIG_COND { get; set; }
        public Nullable<bool> VIS_NEW_ASSE { get; set; }
    
        public virtual ICollection<CLIENT_ASSET> CLIENT_ASSET { get; set; }
        public virtual STATUS_ASSET STATUS_ASSET { get; set; }
        public virtual ICollection<ComponenteStockDetalle> ComponenteStockDetalles { get; set; }
        public virtual ICollection<ComponenteStockCabecera> ComponenteStockCabecera { get; set; }
    }
}

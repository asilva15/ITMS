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
    
    public partial class TYPE_DETAILS_VACATION
    {
        public TYPE_DETAILS_VACATION()
        {
            this.DETAILS_VACATION = new HashSet<DETAILS_VACATION>();
        }
    
        public int ID_TYPE_DETA_VACA { get; set; }
        public string NAM_TYPE_DETA_VACA { get; set; }
        public Nullable<bool> VIG_TYPE_DETA_VACA { get; set; }
    
        public virtual ICollection<DETAILS_VACATION> DETAILS_VACATION { get; set; }
    }
}

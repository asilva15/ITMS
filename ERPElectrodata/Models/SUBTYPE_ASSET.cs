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
    
    public partial class SUBTYPE_ASSET
    {
        public int ID_SUBT_ASSE { get; set; }
        public string NAM_SUBT_ASSE { get; set; }
        public string DES_SUBT_ASSE { get; set; }
        public Nullable<bool> VIG_SUBT_ASSE { get; set; }
        public Nullable<int> ID_TYPE_ASSE { get; set; }
    
        public virtual TYPE_ASSET TYPE_ASSET { get; set; }
    }
}

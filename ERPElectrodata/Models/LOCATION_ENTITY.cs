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
    
    public partial class LOCATION_ENTITY
    {
        public int ID_LOCA { get; set; }
        public Nullable<int> ID_SITE { get; set; }
        public Nullable<int> ID_LOCA_PARENT { get; set; }
        public string NAM_LOCA { get; set; }
        public Nullable<bool> VIG_LOCA { get; set; }
        public Nullable<int> NIVEL { get; set; }
    
        public virtual SITE_ENTITY SITE_ENTITY { get; set; }
    }
}

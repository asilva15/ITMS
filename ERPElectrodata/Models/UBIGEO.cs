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
    
    public partial class UBIGEO
    {
        public UBIGEO()
        {
            this.Gestion_Usuarios = new HashSet<Gestion_Usuarios>();
        }
    
        public int ID_UBIG { get; set; }
        public Nullable<int> ID_UBIG_PARE { get; set; }
        public string COD_UBIG { get; set; }
        public string NAM_UBIG { get; set; }
        public Nullable<int> LEV_UBIG { get; set; }
        public string COD_SEDE { get; set; }
    
        public virtual ICollection<Gestion_Usuarios> Gestion_Usuarios { get; set; }
    }
}

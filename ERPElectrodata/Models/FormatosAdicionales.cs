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
    
    public partial class FormatosAdicionales
    {
        public FormatosAdicionales()
        {
            this.PlantillaAdicionales = new HashSet<PlantillaAdicionales>();
        }
    
        public int Id { get; set; }
        public string NombreFormato { get; set; }
        public Nullable<int> Vigencia { get; set; }
    
        public virtual ICollection<PlantillaAdicionales> PlantillaAdicionales { get; set; }
    }
}

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
    
    public partial class Parametro
    {
        public Parametro()
        {
            this.ParametroDetalle = new HashSet<ParametroDetalle>();
        }
    
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Nullable<bool> Estado { get; set; }
        public Nullable<int> UsuarioRegistra { get; set; }
        public Nullable<System.DateTime> FechaRegistra { get; set; }
        public Nullable<int> UsuarioModifica { get; set; }
        public Nullable<System.DateTime> FechaModifica { get; set; }
    
        public virtual ICollection<ParametroDetalle> ParametroDetalle { get; set; }
    }
}

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
    
    public partial class Plantilla1
    {
        public int Id { get; set; }
        public Nullable<int> IdGrupo { get; set; }
        public string Nombre { get; set; }
        public string Contenido { get; set; }
        public Nullable<int> UsuarioCreacion { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public Nullable<int> UsuarioModifica { get; set; }
        public Nullable<System.DateTime> FechaModifica { get; set; }
        public Nullable<bool> Estado { get; set; }
    }
}

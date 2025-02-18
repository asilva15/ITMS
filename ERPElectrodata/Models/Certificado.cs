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
    
    public partial class Certificado
    {
        public Certificado()
        {
            this.Reprogramacion = new HashSet<Reprogramacion>();
        }
    
        public int Id { get; set; }
        public Nullable<int> IdEstado { get; set; }
        public Nullable<int> IdGerencia { get; set; }
        public Nullable<int> IdArea { get; set; }
        public Nullable<int> IdPersEnti { get; set; }
        public Nullable<int> IdMarca { get; set; }
        public string Nombre { get; set; }
        public Nullable<System.DateTime> FechaProgramada { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public Nullable<int> UsuarioCreacion { get; set; }
        public Nullable<System.DateTime> FechaModifica { get; set; }
        public Nullable<int> UsuarioModifica { get; set; }
        public bool Estado { get; set; }
        public Nullable<decimal> Monto { get; set; }
        public Nullable<int> MotivoId { get; set; }
        public string NombreExamen { get; set; }
        public Nullable<System.DateTime> FechaProgramadaOrigin { get; set; }
        public Nullable<int> JefeInmediato { get; set; }
    
        public virtual EstadoCertificado EstadoCertificado { get; set; }
        public virtual ICollection<Reprogramacion> Reprogramacion { get; set; }
    }
}

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
    
    public partial class PreguntaEncuesta
    {
        public PreguntaEncuesta()
        {
            this.RegistrosEncuesta = new HashSet<RegistrosEncuesta>();
            this.RespuestasEncuesta = new HashSet<RespuestasEncuesta>();
        }
    
        public int IdPregunta { get; set; }
        public Nullable<int> IdCategoria { get; set; }
        public string Pregunta { get; set; }
        public string PreguntaEspecificar { get; set; }
        public System.DateTime FechaRegistro { get; set; }
        public System.DateTime FechaActualizacion { get; set; }
        public Nullable<int> UsuarioRegistra { get; set; }
        public int UsuarioActualizacion { get; set; }
        public bool EstadoPregunta { get; set; }
    
        public virtual CategoriaPreguntaEncuesta CategoriaPreguntaEncuesta { get; set; }
        public virtual ICollection<RegistrosEncuesta> RegistrosEncuesta { get; set; }
        public virtual ICollection<RespuestasEncuesta> RespuestasEncuesta { get; set; }
    }
}

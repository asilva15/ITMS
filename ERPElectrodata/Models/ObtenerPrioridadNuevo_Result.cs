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
    
    public partial class ObtenerPrioridadNuevo_Result
    {
        public int IdPrioridad { get; set; }
        public int TiempoRespuestaMinutos { get; set; }
        public int TiempoAtencionMinutos { get; set; }
        public int TiempoResolucionMinutos { get; set; }
        public Nullable<bool> checkRespuesta { get; set; }
        public Nullable<bool> checkAtencion { get; set; }
        public Nullable<bool> checkResolucion { get; set; }
    }
}

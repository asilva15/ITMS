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
    
    public partial class SLADetalle
    {
        public int Id { get; set; }
        public Nullable<int> IdSLA { get; set; }
        public Nullable<int> IdPrioridad { get; set; }
        public Nullable<decimal> TiempoAtencion { get; set; }
        public Nullable<bool> Estado { get; set; }
        public Nullable<int> TiempoRespuestaMinutos { get; set; }
        public Nullable<int> TiempoAtencionMinutos { get; set; }
        public Nullable<int> TiempoResolucionMinutos { get; set; }
    }
}

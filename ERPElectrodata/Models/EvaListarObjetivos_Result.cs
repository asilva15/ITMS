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
    
    public partial class EvaListarObjetivos_Result
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Peso { get; set; }
        public int IdResultadoEvaluado { get; set; }
        public string ObsResultadoEvaluado { get; set; }
        public int IdResultadoJefe { get; set; }
        public string ObsResultadoJefe { get; set; }
        public int IdEstadoJefe { get; set; }
        public string ObsResultadoRRHH { get; set; }
        public int IdEstadoRRHH { get; set; }
        public int Estado { get; set; }
        public Nullable<int> IdPersEnti { get; set; }
        public Nullable<int> IdPersEntiPadre { get; set; }
    }
}

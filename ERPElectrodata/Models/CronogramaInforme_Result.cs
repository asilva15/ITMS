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
    
    public partial class CronogramaInforme_Result
    {
        public int IdDocuSale { get; set; }
        public int IdInforme { get; set; }
        public string Titulo { get; set; }
        public string OP { get; set; }
        public Nullable<int> IdMarca { get; set; }
        public string Marca { get; set; }
        public string FechaInforme { get; set; }
        public int IdTicket { get; set; }
        public string TipoInforme { get; set; }
        public string Estado { get; set; }
        public string EstadoColor { get; set; }
    }
}

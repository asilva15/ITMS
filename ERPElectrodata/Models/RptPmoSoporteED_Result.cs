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
    
    public partial class RptPmoSoporteED_Result
    {
        public int ID_DOCU_SALE { get; set; }
        public string TipoOP { get; set; }
        public string OP { get; set; }
        public string Compania { get; set; }
        public string FechaCierre { get; set; }
        public string Marca { get; set; }
        public Nullable<int> TiempoSoporte { get; set; }
        public string FinSoporte { get; set; }
        public Nullable<int> BolsaHoras { get; set; }
        public string Observaciones { get; set; }
        public string FechaMantenimiento { get; set; }
        public Nullable<int> MantPeriodico { get; set; }
        public string ClienteFinal { get; set; }
        public string Cliente { get; set; }
        public int ID_MANU { get; set; }
        public string DESC_MANU { get; set; }
    }
}

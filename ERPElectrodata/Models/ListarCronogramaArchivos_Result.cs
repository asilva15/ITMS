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
    
    public partial class ListarCronogramaArchivos_Result
    {
        public int ID { get; set; }
        public Nullable<int> IdDocuSale { get; set; }
        public Nullable<int> IdCronograma { get; set; }
        public string Actividad { get; set; }
        public Nullable<int> IdTipoDocumento { get; set; }
        public string Documento { get; set; }
        public string Link { get; set; }
        public string Archivo { get; set; }
        public string FechaCreacion { get; set; }
        public string FechaActa { get; set; }
        public string Nombre { get; set; }
    }
}

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
    
    public partial class EvaArchivo
    {
        public int Id { get; set; }
        public Nullable<int> IdEvaluacion { get; set; }
        public string NombreArchivo { get; set; }
        public string Extension { get; set; }
        public Nullable<int> UsuarioCrea { get; set; }
        public Nullable<System.DateTime> FechaCrea { get; set; }
        public Nullable<bool> Estado { get; set; }
    
        public virtual EvaEvaluacion EvaEvaluacion { get; set; }
    }
}

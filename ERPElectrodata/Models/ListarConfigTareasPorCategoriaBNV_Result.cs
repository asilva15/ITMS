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
    
    public partial class ListarConfigTareasPorCategoriaBNV_Result
    {
        public Nullable<int> Orden { get; set; }
        public int IdConfigTarea { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public Nullable<int> IdQueu { get; set; }
        public string DES_QUEU { get; set; }
        public Nullable<int> IdAsignado { get; set; }
        public string UsuarioAsignado { get; set; }
        public Nullable<int> IdConfigTareaPadre { get; set; }
        public string TareaPadre { get; set; }
        public Nullable<bool> Estado { get; set; }
    }
}

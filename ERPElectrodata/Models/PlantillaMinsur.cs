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
    
    public partial class PlantillaMinsur
    {
        public int Id { get; set; }
        public Nullable<int> IdAcco { get; set; }
        public Nullable<int> IdGrupo { get; set; }
        public string Nombre { get; set; }
        public string TITLE_TICK { get; set; }
        public Nullable<int> ID_TYPE_TICK { get; set; }
        public Nullable<int> ID_PRIO { get; set; }
        public Nullable<int> ID_SOUR { get; set; }
        public Nullable<int> ID_STAT { get; set; }
        public Nullable<System.DateTime> FEC_INI_TICK { get; set; }
        public Nullable<System.DateTime> FechaRecepcionSolicitud { get; set; }
        public Nullable<int> ID_PERS_ENTI { get; set; }
        public Nullable<int> ID_PERS_ENTI_END { get; set; }
        public Nullable<int> ID_QUEU { get; set; }
        public Nullable<int> ID_PERS_ENTI_ASSI { get; set; }
        public Nullable<int> ID_CATE_N1 { get; set; }
        public Nullable<int> ID_CATE_N2 { get; set; }
        public Nullable<int> ID_CATE { get; set; }
        public Nullable<System.DateTime> FechaIngreso { get; set; }
        public Nullable<bool> REM_CTRL_TICK { get; set; }
        public Nullable<bool> IS_PARENT { get; set; }
        public Nullable<int> ID_TICK_PARENT { get; set; }
        public string SUM_TICK { get; set; }
        public string Adicional1 { get; set; }
        public string Adicional2 { get; set; }
        public Nullable<int> UsuarioCreacion { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public Nullable<int> UsuarioModifica { get; set; }
        public Nullable<System.DateTime> FechaModifica { get; set; }
        public Nullable<bool> Estado { get; set; }
        public Nullable<int> IdGrupal { get; set; }
    }
}

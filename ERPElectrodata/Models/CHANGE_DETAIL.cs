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
    
    public partial class CHANGE_DETAIL
    {
        public int ID { get; set; }
        public Nullable<int> ID_TYPE_TASK { get; set; }
        public Nullable<int> ID_CHANGE_REQUEST { get; set; }
        public Nullable<int> ID_PERSON_ENTI { get; set; }
        public string DETAIL { get; set; }
        public Nullable<System.DateTime> DATE_INIC { get; set; }
        public Nullable<System.DateTime> DATE_END { get; set; }
        public Nullable<bool> STATUS { get; set; }
        public Nullable<System.DateTime> FechaInicioProgramada { get; set; }
        public Nullable<System.DateTime> FechaFinProgramada { get; set; }
        public Nullable<bool> ActividadRealizada { get; set; }
        public Nullable<System.DateTime> FechaActividadRealizada { get; set; }
        public Nullable<bool> ActividadValidada { get; set; }
        public Nullable<System.DateTime> FechaActividadValidada { get; set; }
        public string Motivo { get; set; }
        public Nullable<System.DateTime> FechaEliminacion { get; set; }
        public Nullable<bool> EnvioCorreo { get; set; }
        public Nullable<bool> VisNoti { get; set; }
    
        public virtual CHANGE_TYPE_TASK CHANGE_TYPE_TASK { get; set; }
        public virtual CHANGE_REQUEST CHANGE_REQUEST { get; set; }
    }
}

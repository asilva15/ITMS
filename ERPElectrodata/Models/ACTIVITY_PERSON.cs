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
    
    public partial class ACTIVITY_PERSON
    {
        public int ID_ACTI_PERS { get; set; }
        public string CLI_ACTI_PERS { get; set; }
        public Nullable<System.TimeSpan> HOU_STAR { get; set; }
        public Nullable<System.TimeSpan> HOU_END { get; set; }
        public Nullable<int> ID_PERS_ENTI { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public string DES_ACTI_PERS { get; set; }
        public Nullable<int> ID_TICK { get; set; }
    }
}

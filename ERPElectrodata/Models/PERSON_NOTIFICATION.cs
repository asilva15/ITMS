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
    
    public partial class PERSON_NOTIFICATION
    {
        public PERSON_NOTIFICATION()
        {
            this.PERSON_ENTITY_NOTIFICATION = new HashSet<PERSON_ENTITY_NOTIFICATION>();
        }
    
        public int ID_PERS_NOTI { get; set; }
        public string NAM_PERS_NOTI { get; set; }
        public string DES_PERS_NOTI { get; set; }
    
        public virtual ICollection<PERSON_ENTITY_NOTIFICATION> PERSON_ENTITY_NOTIFICATION { get; set; }
    }
}

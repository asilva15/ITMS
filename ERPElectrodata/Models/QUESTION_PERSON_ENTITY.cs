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
    
    public partial class QUESTION_PERSON_ENTITY
    {
        public int ID_QUES_PERS_ENTI { get; set; }
        public Nullable<int> ID_QUES { get; set; }
        public Nullable<int> ID_PERS_ENTI { get; set; }
        public string VAL_QUES_PERS_ENTI { get; set; }
    
        public virtual QUESTION QUESTION { get; set; }
        public virtual PERSON_ENTITY PERSON_ENTITY { get; set; }
    }
}

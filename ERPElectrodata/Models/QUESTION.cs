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
    
    public partial class QUESTION
    {
        public QUESTION()
        {
            this.QUESTION_OPTION = new HashSet<QUESTION_OPTION>();
            this.QUESTION_PERSON_ENTITY = new HashSet<QUESTION_PERSON_ENTITY>();
        }
    
        public int ID_QUES { get; set; }
        public Nullable<int> ID_QUES_GROU { get; set; }
        public Nullable<int> ID_QUES_TYPE { get; set; }
        public string NAM_QUES { get; set; }
    
        public virtual ICollection<QUESTION_OPTION> QUESTION_OPTION { get; set; }
        public virtual ICollection<QUESTION_PERSON_ENTITY> QUESTION_PERSON_ENTITY { get; set; }
        public virtual QUESTION_GROUP QUESTION_GROUP { get; set; }
        public virtual QUESTION_TYPE QUESTION_TYPE { get; set; }
    }
}

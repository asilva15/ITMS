//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebClientED.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PERSON_ENTITY_NOTIFICATION
    {
        public int ID_PERS_ENTI_NOTI { get; set; }
        public Nullable<int> ID_PERS_ENTI { get; set; }
        public Nullable<int> ID_PERS_NOTI { get; set; }
        public Nullable<int> ID_PERS_CONT { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public Nullable<int> UserId { get; set; }
    
        public virtual PERSON_ENTITY PERSON_ENTITY { get; set; }
    }
}

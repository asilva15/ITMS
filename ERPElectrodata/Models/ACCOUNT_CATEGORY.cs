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
    
    public partial class ACCOUNT_CATEGORY
    {
        public int ID_ACCO_CATE { get; set; }
        public Nullable<int> ID_ACCO { get; set; }
        public Nullable<int> ID_CATE { get; set; }
        public Nullable<int> VIG_CATE { get; set; }
        public Nullable<bool> VIG_ACCO_CATE { get; set; }
        public Nullable<bool> IS_LEVE_TWO { get; set; }
    
        public virtual ACCOUNT ACCOUNT { get; set; }
        public virtual CATEGORY CATEGORY { get; set; }
    }
}

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
    
    public partial class ACCOUNTING_YEAR
    {
        public ACCOUNTING_YEAR()
        {
            this.ACCOUNTING_MONTH = new HashSet<ACCOUNTING_MONTH>();
        }
    
        public int ID_ACCO_YEAR { get; set; }
    
        public virtual ICollection<ACCOUNTING_MONTH> ACCOUNTING_MONTH { get; set; }
    }
}

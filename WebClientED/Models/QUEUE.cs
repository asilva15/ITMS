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
    
    public partial class QUEUE
    {
        public QUEUE()
        {
            this.TICKETs = new HashSet<TICKET>();
        }
    
        public int ID_QUEU { get; set; }
        public string NAM_QUEU { get; set; }
        public string NAM_QUEU_REPO { get; set; }
        public string DES_QUEU { get; set; }
        public Nullable<bool> VIG_QUEU { get; set; }
        public Nullable<bool> VIS_ALL_QUEU { get; set; }
        public Nullable<int> ID_STAT { get; set; }
        public Nullable<int> LEV_QUEU { get; set; }
        public Nullable<bool> VIS_ALL_CHAN { get; set; }
        public string EMA_QUEU { get; set; }
    
        public virtual STATUS STATUS { get; set; }
        public virtual ICollection<TICKET> TICKETs { get; set; }
    }
}

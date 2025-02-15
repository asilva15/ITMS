using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPElectrodata.Models
{
    public class AttachedModel
    {

        public AttachedModel() { 
        
        }

        public int ID_ATTA { get; set; }
        public string EXT_ATTA { get; set; }
        public string NAM_ATTA { get; set; }
        public string NAM_TYPE_DOCU_ATTA { get; set; }
    }
}
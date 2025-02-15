using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electrodata.ERPElectrodata.Domain.Entities.EntityAlternative
{
   public class AttachedEntity
    {
       public AttachedEntity() { }

       public int ID_ATTA { get; set; }
       public string EXT_ATTA { get; set; }
       public string NAM_ATTA { get; set; }
       public string NAM_TYPE_DOCU_ATTA { get; set; }

    }
}

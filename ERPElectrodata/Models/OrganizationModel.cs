using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPElectrodata.Models
{
    public class OrganizationModel
    {
        //cargo
        public string JOB_TITLE { get; set; }
        // Supervisor
        public int ID_BOSS { get; set; }
        public string NAME_BOSS { get; set; }
        // Jefe UEN
        public int ID_BOSS_UEN { get; set; }
        public int ID_CHAR_AREA { get; set; }
        public int ID_CHAR_UEN { get; set; }
        public string NAME_AREA { get; set; }
        public string NAME_UE { get; set; }
    }
}
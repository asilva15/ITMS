using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsoleNotification.Models
{
    public class OrganizationModel
    {
        //cargo
        public string JOB_TITLE { get; set; }        
        //Area
        public int ID_CHAR_AREA { get; set; }
        //Area
        public int ID_CHAR_UEN { get; set; } 
        //Supervisor
        public int ID_BOSS { get; set; }
        public string NAME_BOSS { get; set; }
        //UEN
        public int ID_BOSS_UEN { get; set; }   

    }
}
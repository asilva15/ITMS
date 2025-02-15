using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERPElectrodata.Models
{
    [MetadataType(typeof(ActivityPersonModel))]
    public partial class ACTIVITY_PERSON
    {
        public class ActivityPersonModel
        {
            [Required]
            //[DisplayName("ResourceLanguaje.Resource.Activity")]
            public string DES_ACTI_PERS { get; set; }

            [Required]
            //[DisplayName(ResourceLanguaje.Resource.Since)]
            public TimeSpan HOU_STAR { get; set; }

            [Required]
            //[DisplayName(ResourceLanguaje.Resource.To)]
            public TimeSpan HOU_END { get; set; }

            [Required]
            public int ID_PERS_ENTI { get; set; }

            //[DisplayName(ResourceLanguaje.Resource.Client)]
            public string CLI_ACTI_PERS { get; set; }

            //[DisplayName(ResourceLanguaje.Resource.Ticket)]
            public int ID_TICK { get; set; }
        }
    }

}
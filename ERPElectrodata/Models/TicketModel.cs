using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERPElectrodata.Models
{
    [MetadataType(typeof(TicketModel))]
    public partial class TICKET
    {
        public class TicketModel
        {
            [Required]
            [DisplayName("Date")]
            [DataType(DataType.DateTime)]
            public object FEC_TICK { get; set; }

            [Required]
            public object ID_CATE { get; set; }

            [Required(ErrorMessage = "Field Requierd")]
            [DisplayName("Type")]//[DisplayName("REQUESTER")]
            public int ID_TYPE_TICK { get; set; }

            [Required(ErrorMessage = "Field Requierd")]
            //[DisplayName("Requester")]//[DisplayName("REQUESTER")]
            public int ID_PERS_ENTI { get; set; }

            [Required(ErrorMessage = "Field Requierd")]
            [DisplayName("Affected End User")]//[DisplayName("AFFECTED END USER")]
            public object ID_PERS_ENTI_END { get; set; }

            [Required(ErrorMessage = "Field Requierd")]
            [DisplayName("Queue")]//[DisplayName("AFFECTED END USER")]
            public object ID_QUEU { get; set; }

            [Required(ErrorMessage = "Field Requierd")]
            [DisplayName("Assignee")]//[DisplayName("AFFECTED END USER")]
            public object ID_PERS_ENTI_ASSI { get; set; }

            [Required(ErrorMessage = "Field Requierd")]
            [DisplayName("Summary")]//[DisplayName("AFFECTED END USER")]
            public object SUM_TICK { get; set; }

            [Required(ErrorMessage = "Field Requierd")]
            [DisplayName("Priority")]//[DisplayName("AFFECTED END USER")]
            public object ID_PRIO { get; set; }

            [Required(ErrorMessage = "Field Requierd")]
            [DisplayName("Status")]//[DisplayName("AFFECTED END USER")]
            public object ID_STAT { get; set; }

            [Required(ErrorMessage = "Field Requierd")]
            [DisplayName("Source")]//[DisplayName("AFFECTED END USER")]
            public object ID_SOUR { get; set; }
        }
    }
}
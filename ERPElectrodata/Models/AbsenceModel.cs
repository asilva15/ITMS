using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERPElectrodata.Models
{
    [MetadataType(typeof(AbsenceModel))]
    public partial class ABSENCE
    {
        public class AbsenceModel
        {
            [Required]
            //[DataType(DataType.,ErrorMessage="Type Date")]
            public int ID_TYPE_ABSE { get; set; }

            [Required]
            public int ID_PERS_ENTI { get; set; }

            [Required]
            [DataType(DataType.Date,ErrorMessage="Select Valid Since Date")]
            public DateTime SIN_DATE { get; set; }

            [Required]
            [DataType(DataType.Date,ErrorMessage="Select Valid To Date")]
            public DateTime TO_DATE { get; set; }

            [Required]
            [DataType(DataType.Date, ErrorMessage = "Select Valid Return Data")]
            public DateTime RET_DATE { get; set; }

            [Required]
            //[DataType(DataType.)]
            public string SUM { get; set; }
        }
    }

}
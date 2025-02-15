using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ERPElectrodata.Models
{
    [MetadataType(typeof(SalesOpportunityModel))]
    public partial class SALES_OPPORTUNITY
    {
        public class SalesOpportunityModel
        {
            [Required]
            public string TIT_SALE_OPPO { get; set; }

            [Required]
            public int ID_COMP { get; set; }

            [Required]
            public int ID_PERS_ENTI { get; set; }

            [Required]
            public decimal AMO_SALE_OPPO { get; set; }

            [Required]
            public decimal MAR_SALE_OPPO { get; set; }            

            [Required]
            public int ID_STAT_SALE_OPPO { get; set; }

            [Required]
            public int ID_MANU { get; set; }            

            [Required]
            [DataType(DataType.Date, ErrorMessage = "Select Valid Clossing Date")]
            public DateTime CLO_DATE { get; set; }

            [Required]
            //[DataType(DataType.)]
            public string SUM_SALE_OPPO { get; set; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPElectrodata.Models
{
    
    [ComplexType]
    public class PlantillaBuenaventuraDTO
    {
        public int Id { get; set; }
        public int IdGrupo { get; set; }
        public string Nombre { get; set; }
        public string TITLE_TICK { get; set; }
        public Nullable<int> ID_TYPE_TICK { get; set; }
        public Nullable<int> ID_PRIO { get; set; }
        public Nullable<int> ID_SOUR { get; set; }
        public Nullable<int> ID_STAT { get; set; }
        public Nullable<System.DateTime> FEC_INI_TICK { get; set; }
        public Nullable<System.DateTime> FechaRecepcionSolicitud { get; set; }
        public Nullable<int> IdRazon { get; set; }
        public string TicketExterno { get; set; }
        public Nullable<int> ID_PERS_ENTI { get; set; }
        public Nullable<int> ID_COMP { get; set; }
        public Nullable<int> ID_PERS_ENTI_END { get; set; }
        public Nullable<int> ID_QUEU { get; set; }
        public Nullable<int> ID_PERS_ENTI_ASSI { get; set; }
        public Nullable<int> IdSLA { get; set; }
        public Nullable<int> ID_CATE_N1 { get; set; }
        public Nullable<int> ID_CATE_N2 { get; set; }
        public Nullable<int> ID_CATE_N3 { get; set; }
        public Nullable<int> ID_CATE { get; set; }
        public Nullable<bool> REM_CTRL_TICK { get; set; }
        public Nullable<bool> IS_PARENT { get; set; }
        public Nullable<int> ID_TICK_PARENT { get; set; }
        public Nullable<int> IdGrupoReportador { get; set; }
        public Nullable<int> IdModTrabajo { get; set; }
        public Nullable<int> ID_LOCA { get; set; }
        public Nullable<int> ID_PERS_ENTI_ANLT { get; set; }
        public Nullable<int> IdTipoPase { get; set; }
        public string Solman { get; set; }
        public Nullable<bool> SERVICE { get; set; }
        public Nullable<System.DateTime> FEC_INI_REAL { get; set; }
        public string SUM_TICK { get; set; }
        public string Solucion { get; set; }
    }

}
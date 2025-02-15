using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERPElectrodata.Models
{
    public class PMO
    {
        public int idPM { get; set; }
        public IEnumerable<ListarPmoAsignados_Result> ListarPorCola;

        public int idTipoProyecto { get; set; }
        public IEnumerable<ListarTipoProyecto_Result> ListarTipoProyecto;

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Electrodata.ERPElectrodata.Domain.Entities;

namespace Electrodata.ERPElectrodata.Infra.Reposotories.CatalogoServicioRepositorie
{
    public class CatalogoServicioRepositorie
    {

        public List<CatalogoServicio> Listar()
        {
            List<CatalogoServicio> ListaCatalogoServicio = new List<CatalogoServicio>();

            using (var dbc = new ECoreEntities())
            {
                ListaCatalogoServicio = dbc.CatalogoServicios.Where(x => x.Estado == true).ToList();
            }

            return ListaCatalogoServicio;
        }

    }
}

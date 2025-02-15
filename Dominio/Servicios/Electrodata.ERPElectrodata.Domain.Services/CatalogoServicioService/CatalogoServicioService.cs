using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Electrodata.ERPElectrodata.Domain.Entities;
using Electrodata.ERPElectrodata.Infra.Reposotories.CatalogoServicioRepositorie;

namespace Electrodata.ERPElectrodata.Domain.Services.CatalogoServicioService
{
    public class CatalogoServicioService
    {
        CatalogoServicioRepositorie _repCatalogoServicioRepositorie = new CatalogoServicioRepositorie();

        //public CatalogoServicioService(CatalogoServicioRepositorie repCatalogoServicioRepositorie)
        //{
        //    this._repCatalogoServicioRepositorie = repCatalogoServicioRepositorie;
        //}

        public List<CatalogoServicio> Listar()
        {
            return _repCatalogoServicioRepositorie.Listar();
        }
    }
}

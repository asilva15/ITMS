using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ent = ERPElectrodata.Models;

namespace ERPElectrodata.Object
{
    public class IdiomaDetalle
    {
        private ent.Repositorio<ent.IdiomaDetalle> _repositorio = new ent.Repositorio<ent.IdiomaDetalle>(new ent.EntityEntities());
        public void Adicionar(ent.IdiomaDetalle entidad)
        {
            _repositorio.Adicionar(entidad);
        }

        public void Modificar(ent.IdiomaDetalle entidad)
        {
            _repositorio.Modificar(entidad);
        }

        public void Eliminar(ent.IdiomaDetalle entidad)
        {
            _repositorio.Eliminar(entidad);
        }

        public IEnumerable<ent.IdiomaDetalle> TraerTodo()
        {
            var DetalleQuery = _repositorio.TraerTodo();
            var Resultado = (IEnumerable<ent.IdiomaDetalle>) DetalleQuery;
            return Resultado;
        }

        public ent.IdiomaDetalle TraerUnoPorId(int Id)
        {
            var DetalleQuery = _repositorio.TraerUnoPorId(Id);
            var Resultado = (ent.IdiomaDetalle) DetalleQuery;
            return Resultado;
        }

        public bool ValidarDatosCreacion(string llave,int idIdioma)
        {
            var DetalleQuery = _repositorio.TraerUno(x => x.Llave == llave && x.IdIdioma == idIdioma);
            bool Resultado = DetalleQuery == null ? false : true;
            return Resultado;
        }

        public bool ValidarDatosEdicion(int id, string Nombre, int idIdioma)
        {
            var DetalleQuery = _repositorio.Buscar(x => x.Llave == Nombre && x.Id != id && x.IdIdioma == idIdioma);
            bool Resultado = DetalleQuery.Count() == 0 ? false : true;
            return Resultado;
        }

        public bool ValidarCargaMasiva(int idIdioma, string llave)
        {
            var DetalleQuery = _repositorio.Buscar(x => x.Llave == llave && x.IdIdioma == idIdioma);
            bool Resultado = DetalleQuery.Count() == 0 ? false : true;
            return Resultado;
        }

        public IEnumerable<ent.ListarIdiomaDetalle_Result> Listar(int id)
        {
            var DetalleQuery = new ent.EntityEntities().ListarIdiomaDetalle(id).ToList();
            var Resultado =(IEnumerable<ent.ListarIdiomaDetalle_Result>) DetalleQuery;
            return Resultado;
        }

        public IEnumerable<ent.ListarTextoIdioma_Result> ListarTextoIdioma(String llave)
        {
            var DetalleQuery = new ent.EntityEntities().ListarTextoIdioma(llave);
            var Resultado = (IEnumerable<ent.ListarTextoIdioma_Result>) DetalleQuery;
            return Resultado;
        }

        public IEnumerable<ent.IdiomaDetalle> ListarIdiomaDetalleActivos()
        {
            var DetalleQuery = _repositorio.Buscar(x => x.Estado == true).OrderBy(x => x.Llave);
            var Resultado = (IEnumerable<ent.IdiomaDetalle>) DetalleQuery;
            return Resultado;
        }

        public IEnumerable<ent.ListarCombosIdiomaDetalle_Result> ListarCombosIdiomaDetalle(String llave, String tipo)
        {
            var DetalleQuery = new ent.EntityEntities().ListarCombosIdiomaDetalle(llave, tipo);
            var Resultado = (IEnumerable<ent.ListarCombosIdiomaDetalle_Result>) DetalleQuery;
            return Resultado;
        }
        public IEnumerable<ent.ActualizarValoresIdiomaDetalle_Result> ActualizarValoresIdiomaDetalle(int id, String valor, Boolean estado, int usuario)
        {
            var DetalleQuery = new ent.EntityEntities().ActualizarValoresIdiomaDetalle(id, valor, estado, usuario);
            //var Resultado = Mapper.Map<IEnumerable<ent.ActualizarValoresIdiomaDetalle_Result>, IEnumerable<ent.ActualizarValoresIdiomaDetalle_Result>>(DetalleQuery);
            var Resultado = (IEnumerable<ent.ActualizarValoresIdiomaDetalle_Result>) DetalleQuery;
            return Resultado;
        }
        public IEnumerable<ent.ActualizarTextoIdiomaDetalle_Result> ActualizarTextoIdiomaDetalle(int id, String llave, int usuario, String texto)
        {
            var DetalleQuery = new ent.EntityEntities().ActualizarTextoIdiomaDetalle(id, llave, usuario, texto);
            var Resultado = (IEnumerable<ent.ActualizarTextoIdiomaDetalle_Result>) DetalleQuery;
            return Resultado;
        }
        public List<ent.ListaIdiomaDetalleXML_Result> ListaIdiomaDetalleXML()
        {
            var DetalleQuery = new ent.EntityEntities().ListaIdiomaDetalleXML();
            var Resultado = DetalleQuery.ToList();
            return Resultado;
        }
    }
}

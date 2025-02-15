using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ent = ERPElectrodata.Models;

namespace ERPElectrodata.Object
{
    public class Idioma
    {
        private ent.Repositorio<ent.Idioma> _repositorio = new ent.Repositorio<ent.Idioma>(new ent.EntityEntities());
        public void Adicionar(ent.Idioma entidad)
        {
            _repositorio.Adicionar(entidad);
        }

        public void Modificar(ent.Idioma entidad)
        {
            _repositorio.Modificar(entidad);
        }

        public void Eliminar(ent.Idioma entidad)
        {
            _repositorio.Eliminar(entidad);
        }

        public IEnumerable<ent.Idioma> TraerTodo()
        {
            var DetalleQuery = _repositorio.TraerTodo();
            var Resultado = (IEnumerable<ent.Idioma>) DetalleQuery;
            return Resultado;
        }

        public ent.Idioma TraerUnoPorId(int Id)
        {
            var DetalleQuery = _repositorio.TraerUnoPorId(Id);
            var Resultado = (ent.Idioma) DetalleQuery;
            return Resultado;
        }

        public bool ValidarDatosCreacion(string Nombre)
        {
            var DetalleQuery = _repositorio.TraerUno(x => x.Nombre == Nombre);
            bool Resultado = DetalleQuery == null ? false : true;
            return Resultado;
        }

        public bool ValidarDatosEdicion(int id, string Nombre)
        {
            var DetalleQuery = _repositorio.Buscar(x => x.Nombre == Nombre && x.Id != id);
            bool Resultado = DetalleQuery.Count() == 0 ? false : true;
            return Resultado;
        }

        public IEnumerable<ent.ListarIdioma_Result> Listar(int id)
        {
            var DetalleQuery = new ent.EntityEntities().ListarIdioma(id);
            var Resultado = (IEnumerable<ent.ListarIdioma_Result>) DetalleQuery;
            return Resultado;
        }

        public IEnumerable<ent.ValidarIdiomaPorDefecto_Result> ValidarIdioma(bool principal, bool estado, int id)
        {
            var DetalleQuery = new ent.EntityEntities().ValidarIdiomaPorDefecto(principal,estado,id);
            var Resultado = (IEnumerable<ent.ValidarIdiomaPorDefecto_Result>) DetalleQuery;
            return Resultado;
        }
        public IEnumerable<ent.Idioma> ListarIdiomasActivos()
        {
            var DetalleQuery = _repositorio.Buscar(x => x.Estado == true).OrderBy(x => x.Nombre);
            var Resultado = (IEnumerable<ent.Idioma>) DetalleQuery;
            return Resultado;
        }
        public IEnumerable<ent.ListaComboIdioma_Result> ListaComboIdioma(int id)
        {
            var DetalleQuery = new ent.EntityEntities().ListaComboIdioma(id);
            var Resultado = (IEnumerable<ent.ListaComboIdioma_Result>) DetalleQuery;
            return Resultado;
        }
        public IEnumerable<ent.CambioIdiomadeUsuario_Result> CambioIdiomadeUsuario(int idUsuario,int idIdioma)
        {
            var DetalleQuery = new ent.EntityEntities().CambioIdiomadeUsuario(idUsuario, idIdioma);
            var Resultado = (IEnumerable<ent.CambioIdiomadeUsuario_Result>) DetalleQuery;
            return Resultado;
        }
    }
}

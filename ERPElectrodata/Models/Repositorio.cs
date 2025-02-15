using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPElectrodata.Models
{
    public class Repositorio<T> : IDisposable where T : class
    {
        private readonly DbContext _context;

        public Repositorio(DbContext contexto)
        {
            _context = contexto;
        }

        public IQueryable<T> AsQueryable()
        {
            return _context.Set<T>().AsQueryable();
        }

        public IEnumerable<T> TraerTodo()
        {
            return _context.Set<T>();
        }

        public IEnumerable<T> Buscar(System.Linq.Expressions.Expression<Func<T, bool>> predicado)
        {
            return _context.Set<T>().Where(predicado);
        }

        public T TraerUno(System.Linq.Expressions.Expression<Func<T, bool>> predicado)
        {
            return _context.Set<T>().Where(predicado).FirstOrDefault();
        }

        public T TraerUnoPorId(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Adicionar(T entidad)
        {
            if (_context.Entry<T>(entidad).State != System.Data.Entity.EntityState.Detached)
                _context.Entry<T>(entidad).State = System.Data.Entity.EntityState.Added;
            else
                _context.Set<T>().Add(entidad);
        }

        public void Modificar(T entidad)
        {
            if (_context.Entry<T>(entidad).State != System.Data.Entity.EntityState.Detached)
                _context.Set<T>().Attach(entidad);
            _context.Entry<T>(entidad).State = System.Data.Entity.EntityState.Modified;
        }

        public void Eliminar(T entidad)
        {
            if (_context.Entry<T>(entidad).State == System.Data.Entity.EntityState.Detached)
                _context.Set<T>().Attach(entidad);
            _context.Entry<T>(entidad).State = System.Data.Entity.EntityState.Deleted;
        }

        public void Grabar()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            return;
        }
    }
}
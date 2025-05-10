using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sgi_app.domain.entities;
using sgi_app.infrastructure.sql;

namespace sgi_app.infrastructure.repositories
{
    public interface IProveedorRepository
    {
        IEnumerable<Proveedor> GetAll();
        Proveedor GetById(string id);
        void Add(Proveedor proveedor);
        void Update(Proveedor proveedor);
        void Delete(string id);
    }

    public class ProveedorRepository : IProveedorRepository
    {
        private readonly YourDbContext _context;

        public ProveedorRepository(YourDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Proveedor> GetAll()
        {
            return _context.Proveedores.ToList();
        }

        public Proveedor GetById(string id)
        {
            return _context.Proveedores.Find(id);
        }

        public void Add(Proveedor proveedor)
        {
            // Verificar que el tercero exista
            var tercero = _context.Terceros.Find(proveedor.TerceroId);
            if (tercero == null)
            {
                throw new Exception($"No existe un tercero con el ID: {proveedor.TerceroId}");
            }
            
            _context.Proveedores.Add(proveedor);
            _context.SaveChanges();
        }

        public void Update(Proveedor proveedor)
        {
            _context.Proveedores.Update(proveedor);
            _context.SaveChanges();
        }

        public void Delete(string id)
        {
            var proveedor = GetById(id);
            if (proveedor != null)
            {
                _context.Proveedores.Remove(proveedor);
                _context.SaveChanges();
            }
        }
    }
}
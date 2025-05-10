using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.domain.entities
{
    public class ProductosProveedores
    {
        public string ProductoId { get; set; } // Foreign key
        public int ProveedorId { get; set; } // Foreign key
    }
}
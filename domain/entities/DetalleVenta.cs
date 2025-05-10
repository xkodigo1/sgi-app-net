using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.domain.entities
{
    public class DetalleVenta
    {
        public int Id { get; set; }
        public int VentaId { get; set; } // Foreign key
        public string ProductosId { get; set; } // Foreign key
        public int Cantidad { get; set; }
        public decimal Valor { get; set; }
    }
}
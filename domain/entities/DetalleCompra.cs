using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.domain.entities
{
    public class DetalleCompra
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string ProductoId { get; set; } // Foreign key
        public int Cantidad { get; set; }
        public decimal Valor { get; set; }
        public int CompraId { get; set; } // Foreign key
    }
}
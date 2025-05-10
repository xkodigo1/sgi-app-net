using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.domain.entities
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int FactId { get; set; } // Foreign key
        public string TerceroEnId { get; set; } // Foreign key
        public string TerceroCliId { get; set; } // Foreign key
    }
}
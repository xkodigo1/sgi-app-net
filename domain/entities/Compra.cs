using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.domain.entities
{
    public class Compra
    {
        public int Id { get; set; }
        public string TerceroProvId { get; set; } // Foreign key
        public DateTime Fecha { get; set; }
        public string TerceroEmpId { get; set; } // Foreign key
        public string DocCompra { get; set; }
    }
}
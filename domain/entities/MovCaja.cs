using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.domain.entities
{
    public class MovCaja
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int TipoMov { get; set; } // Foreign key
        public decimal Valor { get; set; }
        public string Concepto { get; set; }
        public string TerceroId { get; set; } // Foreign key
    }
}

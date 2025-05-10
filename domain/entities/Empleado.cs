using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.domain.entities
{
    public class Empleado
    {
        public int Id { get; set; }
        public string TerceroId { get; set; } // Foreign key
        public DateTime FechaIngreso { get; set; }
        public double SalarioBase { get; set; }
        public int EpsId { get; set; } // Foreign key
        public int ArlId { get; set; } // Foreign key
    }
}
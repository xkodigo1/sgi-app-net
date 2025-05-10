using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.domain.entities
{
    public class PlanProducto
    {
        public int PlanId { get; set; } // Foreign key
        public string ProductoId { get; set; } // Foreign key
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.domain.entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string TerceroId { get; set; } // Foreign key
        public string Nombre { get; set; }
        public string Email { get; set; } 
        public DateTime FechaNac { get; set; }
        public DateTime FechaCompra { get; set; }
    }
}
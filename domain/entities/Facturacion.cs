using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sgi_app.domain.entities
{
    public class Facturacion
    {
        public int Id { get; set; }
        public string FechaResolucion { get; set; }
        public int Numero { get; set; }
        public int NumFinal { get; set; }
        public DateTime FechaActual { get; set; }
    }
}
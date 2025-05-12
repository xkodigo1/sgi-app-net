using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("Planes")]
    public class Plan
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public double Dcto { get; set; }

        public virtual ICollection<PlanProducto> PlanProductos { get; set; }
    }
}
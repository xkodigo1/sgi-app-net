using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("plan_Producto")]
    public class PlanProducto
    {
        [Column("Plan_id")]
        public int PlanId { get; set; }

        [Column("Producto_id")]
        public string ProductoId { get; set; }

        [ForeignKey("PlanId")]
        public Plan Plan { get; set; }

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
    }
}
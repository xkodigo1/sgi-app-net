using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("detalle_venta")]
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }
        
        [Column("venta_id")]
        public int VentaId { get; set; }
        
        [Column("Productos_id")]
        public string ProductosId { get; set; }
        
        public int Cantidad { get; set; }
        
        public decimal Valor { get; set; }
    }
}
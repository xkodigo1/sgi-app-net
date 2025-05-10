using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("detalle_compra")]
    public class DetalleCompra
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime Fecha { get; set; }
        
        [Column("Producto_id")]
        public string ProductoId { get; set; }
        
        public int Cantidad { get; set; }
        
        public decimal Valor { get; set; }
        
        [Column("Compra_id")]
        public int CompraId { get; set; }
    }
}
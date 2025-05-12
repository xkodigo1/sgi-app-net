using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("Productos")]
    public class Producto
    {
        [Key]
        public string Id { get; set; }
        
        public string Nombre { get; set; }
        
        public int Stock { get; set; }
        
        public int StockMin { get; set; }
        
        public int StockMax { get; set; }
        
        [Column("Precio", TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
    }
}
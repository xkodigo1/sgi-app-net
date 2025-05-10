using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("Venta")]
    public class Venta
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime Fecha { get; set; }
        
        [Column("Fact_id")]
        public int FactId { get; set; }
        
        [Column("TerceroEn_id")]
        public string TerceroEnId { get; set; }
        
        [Column("TerceroCli_id")]
        public string TerceroCliId { get; set; }
    }
}
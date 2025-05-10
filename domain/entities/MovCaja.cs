using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("MovCaja")]
    public class MovCaja
    {
        [Key]
        public int Id { get; set; }
        
        public DateTime Fecha { get; set; }
        
        [Column("TipoMov")]
        public int TipoMovId { get; set; }
        
        public decimal Valor { get; set; }
        
        public string Concepto { get; set; }
        
        [Column("Tercero_id")]
        public string TerceroId { get; set; }
    }
}

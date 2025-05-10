using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("Compras")]
    public class Compra
    {
        [Key]
        public int Id { get; set; }
        
        [Column("TerceroProv_id")]
        public string TerceroProvId { get; set; }
        
        public DateTime Fecha { get; set; }
        
        [Column("TerceroEmp_id")]
        public string TerceroEmpId { get; set; }
        
        public string DocCompra { get; set; }
    }
}
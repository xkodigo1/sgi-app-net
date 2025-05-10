using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("Proveedores")]
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }
        
        [Column("Tercero_id")]
        public string TerceroId { get; set; }
        
        public double Dcto { get; set; }
        
        public int DiaPago { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        
        [Column("Tercero_id")]
        public string TerceroId { get; set; }
        
        public DateTime FechaNac { get; set; }
        
        public DateTime FechaCompra { get; set; }
    }
}
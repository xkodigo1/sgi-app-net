using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("Ciudad")]
    public class Ciudad
    {
        [Key]
        public int Id { get; set; }
        
        public string Nombre { get; set; }
        
        [Column("Region_id")]
        public int RegionId { get; set; } // Foreign key
    }
}
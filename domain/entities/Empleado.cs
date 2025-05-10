using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("Empleados")]
    public class Empleado
    {
        [Key]
        public int Id { get; set; }
        
        [Column("Tercero_id")]
        public string TerceroId { get; set; }
        
        public DateTime FechaIngreso { get; set; }
        
        public double SalarioBase { get; set; }
        
        [Column("Eps_id")]
        public int EpsId { get; set; }
        
        [Column("arl_id")]
        public int ArlId { get; set; }
    }
}
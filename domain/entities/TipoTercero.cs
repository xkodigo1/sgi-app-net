using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("tipo_terceros")]
    public class TipoTercero
    {
        [Key]
        public int Id { get; set; }
        
        [Column("descripcion")]
        public string Descripcion { get; set; }
    }
}
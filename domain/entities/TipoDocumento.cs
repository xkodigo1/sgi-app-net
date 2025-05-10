using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("tipo_documentos")]
    public class TipoDocumento
    {
        [Key]
        public int Id { get; set; }
        
        public string Descripcion { get; set; }
    }
}
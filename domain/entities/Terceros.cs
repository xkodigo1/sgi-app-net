using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sgi_app.domain.entities
{
    [Table("Terceros")]
    public class Terceros
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellidos { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Column("TipoDoc_id")]
        public int TipoDocId { get; set; }

        [Column("TipoTercero_id")]
        public int TipoTerceroId { get; set; }

        [Column("Ciudad_id")]
        public int CiudadId { get; set; }
    }
}
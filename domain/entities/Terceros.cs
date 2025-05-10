using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace sgi_app.domain.entities
{
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
    }
}
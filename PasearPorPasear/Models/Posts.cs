using System;
using System.ComponentModel.DataAnnotations;

namespace PasearPorPasear.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Desarrollo { get; set; }

        [Required]
        public DateTime FechaEvento { get; set; }
    }
}

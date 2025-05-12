using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAlcaraz.Models
{
    public class Inmueble   
    {   
        [Key]
        public int IdInmueble { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Direccion { get; set; }

        [Required]
        public int IdTipo { get; set; }

      

        [Required]
        public int IdUso{ get; set; }

        [Required]
        public int IdPropietario { get; set; }

        [ForeignKey(nameof(IdPropietario))]
        public Propietario? Duenio { get; set; }

        [Required]
        public int Ambientes { get; set; }

        public decimal Longitud { get; set; }
        public decimal Latitud { get; set; }

        public int Superficie { get; set; }
        
        [Required]
        public int Disponible { get; set; }

        [Required]

        public decimal Precio { get; set; }

        public string? PortadaUrl { get; set; }

        [NotMapped]
        public IFormFile? PortadaFile { get; set; }

        [ForeignKey(nameof(IdInmueble))]
        public IList<Imagen> Imagenes { get; set; } = new List<Imagen>();
    }
}
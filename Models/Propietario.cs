using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAlcaraz.Models
{
    public class Propietario
    {
        [Key]
        public int IdPropietario { get; set; }
        
        [Required]
        [StringLength(15)]
        public string Dni { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }

        [StringLength(50)]
        public string Telefono { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [StringLength(50)]
        public string Direccion { get; set; }

        public override string ToString(){

            var res = $"{Nombre} {Apellido}";
			if(!String.IsNullOrEmpty(Dni)) {
				res += $" ({Dni})";
			}
			return res;
        }

        
}
}
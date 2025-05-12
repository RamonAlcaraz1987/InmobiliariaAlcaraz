using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAlcaraz.Models
{
    public class Imagen
    {
        [Key]
        public int IdImagen { get; set; }
        public string Url { get; set; }
        public int IdInmueble { get; set; }
        public IFormFile? Archivo { get; set; }=null;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAlcaraz.Models
{
    public class Uso
    {
    [Key]
    public int IdUso { get; set; }
    public string Descripcion { get; set; }
    }
}
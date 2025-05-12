using System;
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAlcaraz.Models
{
    public class BusquedaContratosViewModel
    {
        
        [Display(Name = "Desde")]
        [DataType(DataType.Date)]
        public DateTime? FechaDesde { get; set; }

        
        [Display(Name = "Hasta")]
        [DataType(DataType.Date)]
        public DateTime? FechaHasta { get; set; }

        public int? PlazoDias { get; set; }
    }
}
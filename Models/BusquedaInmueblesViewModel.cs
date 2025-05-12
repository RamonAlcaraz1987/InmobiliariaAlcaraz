using System;
using System.ComponentModel.DataAnnotations;

namespace InmobiliariaAlcaraz.Models
{
    public class BusquedaInmueblesViewModel
    {
        
        
        public DateTime? FechaDesde { get; set; }

       
        public DateTime? FechaHasta { get; set; }
    }
}
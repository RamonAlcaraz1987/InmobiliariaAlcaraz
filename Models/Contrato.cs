using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAlcaraz.Models
{
    public class Contrato
    {
        [Key]
        [Display(Name = "N° Contrato")]
        public int IdContrato { get; set; }

        [Required]
        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }

        [ForeignKey(nameof(IdInquilino))]
        public Inquilino? Inquilino { get; set; }

        [Required]
        [Display(Name = "Inmueble")]
        public int IdInmueble { get; set; }

        [ForeignKey(nameof(IdInmueble))]
        public Inmueble? Inmueble { get; set; }

        [Required]
        [Display(Name = "Monto Mensual")]
        [DataType(DataType.Currency)]
        public decimal MontoMensual { get; set; }

        [Required]
        [Display(Name = "Fecha Inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required]
        [Display(Name = "Fecha Fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Required]
        [Display(Name = "Creado por")]
        public int IdUsuarioCreacion { get; set; }

        [ForeignKey(nameof(IdUsuarioCreacion))]
        public Usuario? UsuarioCreacion { get; set; }

        [Display(Name = "Finalizado por")]
        public int? IdUsuarioFinalizacion { get; set; }

        [ForeignKey(nameof(IdUsuarioFinalizacion))]
        public Usuario? UsuarioFinalizacion { get; set; }

        [Display(Name = "Fecha Creación")]
        [DataType(DataType.DateTime)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha Fin Anticipado")]
        [DataType(DataType.Date)]
        public DateTime? FechaFinAnticipado { get; set; } = null;

        [Display(Name = "Pagos Esperados")]
        public int PagosEsperados { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public int Estado { get; set; } = 1; // 1 = Activo, 0 = Inactivo
    }
}
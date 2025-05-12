using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaAlcaraz.Models
{
    public class Pago
    {
        [Key]
        [Display(Name = "N° Pago")]
        public int IdPago { get; set; }

        [Required]
        [Display(Name = "Contrato")]
        public int IdContrato { get; set; }

        [ForeignKey(nameof(IdContrato))]
        public Contrato? Contrato { get; set; }

        [Required]
        [Display(Name = "Monto")]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero")]
        public decimal Monto { get; set; }

        [Display(Name = "Fecha de Pago")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaPago { get; set; }

        [Required]
        [Display(Name = "N° Pago")]
        public int NumeroPago { get; set; }

        [Required]
        [Display(Name = "Detalle")]
        [StringLength(255)]
        public string Detalle { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Anulado")]
        public bool Anulado { get; set; } = false;

        [Required]
        [Display(Name = "Creado por")]
        public int IdUsuarioCreacion { get; set; }

        [ForeignKey(nameof(IdUsuarioCreacion))]
        public Usuario? UsuarioCreacion { get; set; }

        [Display(Name = "Anulado por")]
        public int? IdUsuarioAnulacion { get; set; }

        [ForeignKey(nameof(IdUsuarioAnulacion))]
        public Usuario? UsuarioAnulacion { get; set; }

        [Display(Name = "Fecha Creación")]
        [DataType(DataType.DateTime)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha Anulación")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaAnulacion { get; set; }

        [Required]
        [Display(Name = "Es Multa")]
        public int EsMulta { get; set; } = 0; // 0 = No, 1 = Sí

    }
}
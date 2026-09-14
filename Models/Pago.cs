using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class Pago
    {
        [Key]
        [Display(Name = "Código")]
        public int IdPago { get; set; }

        [Required(ErrorMessage = "Debe asociar el pago a una reserva")]
        [Display(Name = "Reserva")]
        public int IdReserva { get; set; }

        [ForeignKey(nameof(IdReserva))]
        public Reserva? Reserva { get; set; }

        [Display(Name = "N° de Pago")]
        public int NumeroPago { get; set; } = 1;

        [Required(ErrorMessage = "La fecha de pago es requerida")]
        [Display(Name = "Fecha de Pago")]
        [DataType(DataType.DateTime)]
        public DateTime FechaPago { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El importe es requerido")]
        [Range(0.01, 1000000000, ErrorMessage = "El importe debe ser mayor a 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Importe")]
        public decimal Importe { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        [StringLength(255, ErrorMessage = "El concepto no puede superar 255 caracteres")]
        [Display(Name = "Concepto")]
        public string Concepto { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Activo";

        [Display(Name = "Creado por (Usuario)")]
        public int? IdUsuarioCreador { get; set; }

        [ForeignKey(nameof(IdUsuarioCreador))]
        public Usuario? UsuarioCreador { get; set; }

        [Display(Name = "Anulado por (Usuario)")]
        public int? IdUsuarioAnulador { get; set; }

        [ForeignKey(nameof(IdUsuarioAnulador))]
        public Usuario? UsuarioAnulador { get; set; }

        [Display(Name = "Fecha de Anulación")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaAnulacion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}

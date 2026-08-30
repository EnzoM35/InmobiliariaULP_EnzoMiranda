using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class Reserva
    {
        [Key]
        [Display(Name = "Código")]
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inquilino")]
        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }

        [ForeignKey(nameof(IdInquilino))]
        public Inquilino? Inquilino { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inmueble")]
        [Display(Name = "Inmueble")]
        public int IdInmueble { get; set; }

        [ForeignKey(nameof(IdInmueble))]
        public Inmueble? Inmueble { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Desde")]
        public DateTime FechaDesde { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Hasta")]
        public DateTime FechaHasta { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "El precio por día es obligatorio")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio por Día")]
        public decimal PrecioPorDia { get; set; }

        [Required(ErrorMessage = "El monto total es obligatorio")]
        [DataType(DataType.Currency)]
        [Display(Name = "Monto Total")]
        public decimal MontoTotal { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha Terminación Efectiva")]
        public DateTime? FechaTerminacion { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Multa")]
        public decimal Multa { get; set; } = 0;

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Vigente";

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [NotMapped]
        [Display(Name = "Cantidad de Días")]
        public int CantidadDias => (FechaHasta - FechaDesde).Days > 0 ? (FechaHasta - FechaDesde).Days : 0;

        public override string ToString()
        {
            return $"Reserva #{IdReserva} - Inmueble: {Inmueble?.Direccion} (Inquilino: {Inquilino?.Nombre} {Inquilino?.Apellido})";
        }
    }
}

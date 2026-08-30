using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class Inmueble
    {
        [Key]
        [Display(Name = "Código")]
        public int IdInmueble { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(150, ErrorMessage = "La dirección no puede superar los 150 caracteres")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = "";

        [Required(ErrorMessage = "El cupo es obligatorio")]
        [Range(1, 50, ErrorMessage = "El cupo debe ser de al menos 1 persona")]
        [Display(Name = "Cupo (personas)")]
        public int Cupo { get; set; } = 1;

        [Display(Name = "Latitud")]
        public decimal? Latitud { get; set; }

        [Display(Name = "Longitud")]
        public decimal? Longitud { get; set; }

        [Required(ErrorMessage = "El precio por día es obligatorio")]
        [Range(0.01, 10000000.0, ErrorMessage = "El precio por día debe ser mayor a 0")]
        [DataType(DataType.Currency)]
        [Display(Name = "Precio por Día")]
        public decimal PrecioDia { get; set; }

        [Required(ErrorMessage = "El porcentaje de reserva es obligatorio")]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0% y 100%")]
        [Display(Name = "Porcentaje de Reserva (%)")]
        public decimal PorcentajeReserva { get; set; } = 10.00m;

        [Display(Name = "Disponible")]
        public bool Disponible { get; set; } = true;

        [Display(Name = "Imagen de Portada")]
        public string? Portada { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de inmueble")]
        [Display(Name = "Tipo de Inmueble")]
        public int IdTipoInmueble { get; set; }

        [ForeignKey(nameof(IdTipoInmueble))]
        public TipoInmueble? Tipo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un propietario")]
        [Display(Name = "Propietario")]
        public int IdPropietario { get; set; }

        [ForeignKey(nameof(IdPropietario))]
        public Propietario? Duenio { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        public override string ToString()
        {
            return $"{Direccion} - {Tipo?.Descripcion} (Dueño: {Duenio?.Nombre} {Duenio?.Apellido})";
        }
    }
}

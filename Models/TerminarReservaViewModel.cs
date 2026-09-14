using System.ComponentModel.DataAnnotations;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class TerminarReservaViewModel
    {
        public int IdReserva { get; set; }
        public Reserva? Reserva { get; set; }

        [Required(ErrorMessage = "La fecha de terminación es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Efectiva de Terminación")]
        public DateTime FechaTerminacion { get; set; } = DateTime.Today;

        [Display(Name = "Días Originales Pactados")]
        public int DiasPactados { get; set; }

        [Display(Name = "Días Efectivamente Cumplidos")]
        public int DiasCumplidos { get; set; }

        [Display(Name = "Días Restantes No Utilizados")]
        public int DiasRestantes { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Saldo por Días No Utilizados")]
        public decimal SaldoNoUtilizado { get; set; }

        [Display(Name = "% de Multa Aplicable")]
        public decimal PorcentajeMulta { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Monto de la Multa")]
        public decimal MultaCalculada { get; set; }
    }
}

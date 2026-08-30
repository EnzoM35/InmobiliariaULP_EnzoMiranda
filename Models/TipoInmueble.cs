using System.ComponentModel.DataAnnotations;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class TipoInmueble
    {
        [Key]
        [Display(Name = "Código")]
        public int IdTipoInmueble { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(50, ErrorMessage = "La descripción no puede superar los 50 caracteres")]
        [Display(Name = "Tipo de Inmueble")]
        public string Descripcion { get; set; } = "";

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        public override string ToString()
        {
            return Descripcion;
        }
    }
}

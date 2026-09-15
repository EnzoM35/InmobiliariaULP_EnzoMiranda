using System.ComponentModel.DataAnnotations;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class ImagenInmueble
    {
        [Key]
        public int IdImagen { get; set; }

        [Required]
        public int IdInmueble { get; set; }

        [Required(ErrorMessage = "La ruta de la imagen es obligatoria")]
        [StringLength(255)]
        [Display(Name = "Ruta de la imagen")]
        public string Url { get; set; } = "";

        [Display(Name = "Es portada")]
        public bool EsPortada { get; set; }
    }
}

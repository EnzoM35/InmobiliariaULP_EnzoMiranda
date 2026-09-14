using System.ComponentModel.DataAnnotations;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo electrÃ³nico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo invÃ¡lido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La contraseÃ±a es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Recordarme")]
        public bool Recordarme { get; set; } = false;

        public string? ReturnUrl { get; set; }
    }
}
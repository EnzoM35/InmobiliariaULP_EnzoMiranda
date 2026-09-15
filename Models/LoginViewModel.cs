using System.ComponentModel.DataAnnotations;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Recordarme")]
        public bool Recordarme { get; set; } = false;

        public string? ReturnUrl { get; set; }
    }
}
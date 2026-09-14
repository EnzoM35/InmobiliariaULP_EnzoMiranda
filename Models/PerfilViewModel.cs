using System.ComponentModel.DataAnnotations;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class PerfilViewModel
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de correo invÃ¡lido")]
        public string Email { get; set; } = "";

        public string Rol { get; set; } = "";

        public string? AvatarUrl { get; set; }

        [Display(Name = "Nueva ContraseÃ±a (dejar en blanco para no cambiar)")]
        [DataType(DataType.Password)]
        public string? NuevaPassword { get; set; }

        [Display(Name = "Confirmar ContraseÃ±a")]
        [DataType(DataType.Password)]
        [Compare("NuevaPassword", ErrorMessage = "Las contraseÃ±as no coinciden")]
        public string? ConfirmarPassword { get; set; }

        [Display(Name = "Archivo de Avatar")]
        public IFormFile? AvatarFile { get; set; }
    }
}
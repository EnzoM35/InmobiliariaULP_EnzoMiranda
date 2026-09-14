using System.ComponentModel.DataAnnotations;

namespace Laboratorio_II___Proyecto_Inmobiliaria_EnzoMiranda.Models
{
    public class Usuario
    {
        [Key]
        [Display(Name = "CÃ³digo")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50)]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email invÃ¡lido")]
        [StringLength(100)]
        public string Email { get; set; } = "";

        [Display(Name = "ContraseÃ±a")]
        public string? Password { get; set; }

        public string PasswordHash { get; set; } = "";

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Rol { get; set; } = "Empleado"; // "Administrador" o "Empleado"

        [Display(Name = "Avatar / Foto")]
        public string? AvatarUrl { get; set; }

        public bool Activo { get; set; } = true;

        public override string ToString() => $"{Nombre} {Apellido} ({Rol})";
    }
}
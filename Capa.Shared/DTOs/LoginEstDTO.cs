using System.ComponentModel.DataAnnotations;

namespace Capa.Shared.DTOs
{
    public class LoginEstDTO
    {
        [Required]
        public string Correo { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Clave { get; set; } = null!;
    }
}

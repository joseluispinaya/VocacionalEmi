using System.ComponentModel.DataAnnotations;

namespace Capa.Shared.Entities
{
    public class Estudiante
    {
        public int Id { get; set; }

        [MaxLength(10, ErrorMessage = "El campo Nro Cedula debe tener máximo 10 caractéres.")]
        [Required(ErrorMessage = "El campo Nro Cedula es obligatorio.")]
        public string NroCi { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "El campo Nombres debe tener máximo 50 caracteres.")]
        [Required(ErrorMessage = "El campo Nombres es obligatorio.")]
        public string Nombres { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "El campo Apellidos debe tener máximo 50 caracteres.")]
        [Required(ErrorMessage = "El campo Apellidos es obligatorio.")]
        public string Apellidos { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "El campo Correo debe tener máximo 50 caracteres.")]
        [Required(ErrorMessage = "El campo Correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debes ingresar un correo válido.")]
        public string Correo { get; set; } = null!;

        [Required(ErrorMessage = "El campo Clave es obligatorio.")]
        public string Clave { get; set; } = null!;
        public string? Photo { get; set; }
        public UnidadEducativa UnidadEducativa { get; set; } = null!;
        public int UnidadEducativaId { get; set; }
        public ICollection<IntentoTest> IntentosTest { get; set; } = [];
    }
}

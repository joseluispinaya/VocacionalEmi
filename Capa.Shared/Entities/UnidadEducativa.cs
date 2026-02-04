using System.ComponentModel.DataAnnotations;

namespace Capa.Shared.Entities
{
    public class UnidadEducativa
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "El campo Nombre debe tener máximo 100 caractéres.")]
        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "El campo Responsable debe tener máximo 100 caractéres.")]
        [Required(ErrorMessage = "El campo Responsable es obligatorio.")]
        public string Responsable { get; set; } = null!;

        [MaxLength(10, ErrorMessage = "El campo Nro Contacto debe tener máximo 10 caractéres.")]
        [Required(ErrorMessage = "El campo Nro Contacto es obligatorio.")]
        public string NroContacto { get; set; } = null!;
        public string? Ubicacion { get; set; }
        public ICollection<Estudiante> Estudiantes { get; set; } = [];
    }
}

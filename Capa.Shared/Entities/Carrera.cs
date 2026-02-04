using System.ComponentModel.DataAnnotations;

namespace Capa.Shared.Entities
{
    public class Carrera
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "El campo Nombre debe tener máximo 100 caractéres.")]
        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        public string Nombre { get; set; } = null!;
        public ICollection<Recomendacion> Recomendaciones { get; set; } = [];
    }
}

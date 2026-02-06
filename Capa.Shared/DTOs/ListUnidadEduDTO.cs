namespace Capa.Shared.DTOs
{
    public class ListUnidadEduDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Responsable { get; set; } = null!;
        public string NroContacto { get; set; } = null!;
        public string? Ubicacion { get; set; }
        public int CantidadEstudiantes { get; set; }
        public string CantStr => $"{CantidadEstudiantes} Estudiante{(CantidadEstudiantes == 1 ? "" : "s")}";
    }
}

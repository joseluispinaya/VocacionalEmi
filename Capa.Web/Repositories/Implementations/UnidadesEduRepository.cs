using Capa.Shared.DTOs;
using Capa.Shared.Responses;
using Capa.Web.Data;
using Capa.Web.Repositories.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace Capa.Web.Repositories.Implementations
{
    public class UnidadesEduRepository : IUnidadesEduRepository
    {
        private readonly DataContext _context;
        public UnidadesEduRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ActionResponse<IEnumerable<ListUnidadEduDTO>>> GetListAsync()
        {
            var unidades = await _context.UnidadEducativas
                .Select(u => new ListUnidadEduDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Responsable = u.Responsable,
                    NroContacto = u.NroContacto,
                    Ubicacion = u.Ubicacion,
                    CantidadEstudiantes = u.Estudiantes.Count()
                }).ToListAsync();

            if (unidades.Count == 0)
            {
                return new ActionResponse<IEnumerable<ListUnidadEduDTO>>
                {
                    WasSuccess = false,
                    Message = "No se encontraron unidades educativas.",
                    Result = null
                };
            }

            return new ActionResponse<IEnumerable<ListUnidadEduDTO>>
            {
                WasSuccess = true,
                Message = "Unidades educativas obtenidas exitosamente.",
                Result = unidades
            };
        }
    }
}

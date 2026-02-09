using Capa.Shared.DTOs;
using Capa.Shared.Responses;
using Capa.Web.Data;
using Capa.Web.Repositories.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace Capa.Web.Repositories.Implementations
{
    public class PreguntasRepository : IPreguntasRepository
    {
        private readonly DataContext _context;

        public PreguntasRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ActionResponse<IReadOnlyList<PreguntaItemDTO>>> GetDetalleAsync(int cuestionarioId)
        {
            var preguntas = await _context.Preguntas
                .AsNoTracking()
                .Where(p => p.CuestionarioId == cuestionarioId)
                .Select(p => new PreguntaItemDTO
                {
                    Id = p.Id,
                    Texto = p.Texto,
                    CuestionarioId = p.CuestionarioId
                })
                .ToListAsync();

            if (preguntas.Count == 0)
            {
                return new ActionResponse<IReadOnlyList<PreguntaItemDTO>>
                {
                    WasSuccess = false,
                    Message = "No existen preguntas disponibles para el cuestionario."
                };
            }

            return new ActionResponse<IReadOnlyList<PreguntaItemDTO>>
            {
                WasSuccess = true,
                Result = preguntas
            };
        }

        public async Task<ActionResponse<IReadOnlyList<PreguntaItemDTO>>> GetRandomAsync(int cantidad = 7)
        {
            var preguntas = await _context.Preguntas
                .AsNoTracking()
                .OrderBy(p => Guid.NewGuid())
                .Take(cantidad)
                .Select(p => new PreguntaItemDTO
                {
                    Id = p.Id,
                    Texto = p.Texto,
                    CuestionarioId = p.CuestionarioId
                })
                .ToListAsync();

            if (preguntas.Count == 0)
            {
                return new ActionResponse<IReadOnlyList<PreguntaItemDTO>>
                {
                    WasSuccess = false,
                    Message = "No existen preguntas disponibles."
                };
            }

            return new ActionResponse<IReadOnlyList<PreguntaItemDTO>>
            {
                WasSuccess = true,
                Result = preguntas
            };
        }
    }
}

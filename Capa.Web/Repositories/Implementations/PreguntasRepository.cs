using Capa.Shared.DTOs;
using Capa.Shared.Entities;
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

        public async Task<ActionResponse<bool>> AddAsync(PreguntaItemDTO preguntaItemDTO)
        {
            try
            {
                var cuestio = await _context.Cuestionarios.FindAsync(preguntaItemDTO.CuestionarioId);
                if (cuestio == null)
                {
                    return new ActionResponse<bool> { WasSuccess = false, Message = "Cuestionario no encontrada." };
                }

                var pregunta = new Pregunta
                {
                    Texto = preguntaItemDTO.Texto,
                    CuestionarioId = preguntaItemDTO.CuestionarioId
                };
                _context.Add(pregunta);
                await _context.SaveChangesAsync();

                return new ActionResponse<bool> { WasSuccess = true, Result = true, Message = "Pregunta registrado correctamente." };
            }
            catch (Exception)
            {
                return new ActionResponse<bool> { WasSuccess = false, Message = "Ocurrio un problema intente mas tarde." };
            }
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

        public async Task<ActionResponse<bool>> UpdateAsync(PreguntaItemDTO preguntaItemDTO)
        {
            try // <--- Mueve el try al inicio
            {
                var preguntaModel = await _context.Preguntas.FindAsync(preguntaItemDTO.Id);
                if (preguntaModel == null)
                {
                    return new ActionResponse<bool> { WasSuccess = false, Message = "Registro no encontrado." };
                }

                preguntaModel.Texto = preguntaItemDTO.Texto;
                preguntaModel.CuestionarioId = preguntaItemDTO.CuestionarioId;

                await _context.SaveChangesAsync(); // EF Core detecta cambios automáticamente

                return new ActionResponse<bool> { WasSuccess = true, Result = true, Message = "Pregunta actualizada correctamente." };
            }
            catch (Exception)
            {
                // Opcional: Loguear el error real 'ex' aquí
                return new ActionResponse<bool> { WasSuccess = false, Message = "Ocurrió un error al actualizar." };
            }
        }
    }
}

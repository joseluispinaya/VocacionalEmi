using Capa.Shared.DTOs;
using Capa.Shared.Responses;

namespace Capa.Web.Repositories.Intefaces
{
    public interface IPreguntasRepository
    {
        Task<ActionResponse<bool>> AddAsync(PreguntaItemDTO preguntaItemDTO);
        Task<ActionResponse<bool>> UpdateAsync(PreguntaItemDTO preguntaItemDTO);
        Task<ActionResponse<IReadOnlyList<PreguntaItemDTO>>> GetRandomAsync(int cantidad = 7);
        Task<ActionResponse<IReadOnlyList<PreguntaItemDTO>>> GetDetalleAsync(int cuestionarioId);
        // Task<ActionResponse<bool>> UpdateAsync(PreguntaItemDTO preguntaItemDTO);
    }
}

using Capa.Shared.DTOs;
using Capa.Shared.Responses;

namespace Capa.Web.Repositories.Intefaces
{
    public interface ICuestionariosRepository
    {
        Task<ActionResponse<IEnumerable<CuestionarioDTO>>> GetListAsync();
        Task<ActionResponse<bool>> AddAsync(CuestionarioDTO cuestionarioDTO);
        Task<ActionResponse<bool>> UpdateAsync(CuestionarioDTO cuestionarioDTO);
        // Task<ActionResponse<bool>> AddAsync(CuestionarioDTO cuestionarioDTO);
    }
}

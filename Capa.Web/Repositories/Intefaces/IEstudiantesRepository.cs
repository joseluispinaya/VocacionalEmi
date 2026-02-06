using Capa.Shared.DTOs;
using Capa.Shared.Responses;
using Capa.Web.Models;

namespace Capa.Web.Repositories.Intefaces
{
    public interface IEstudiantesRepository
    {
        Task<ActionResponse<bool>> AddAsync(EstudianteDTO estudianteDTO);
        Task<ActionResponse<IEnumerable<ListEstDTO>>> GetListAsync();
    }
}

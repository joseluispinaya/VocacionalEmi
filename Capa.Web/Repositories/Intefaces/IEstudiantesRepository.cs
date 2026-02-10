using Capa.Shared.DTOs;
using Capa.Shared.Responses;
using Capa.Web.Models;

namespace Capa.Web.Repositories.Intefaces
{
    public interface IEstudiantesRepository
    {
        Task<ActionResponse<bool>> AddAsync(EstudianteDTO estudianteDTO);
        Task<ActionResponse<bool>> UpdateAsync(EstudianteDTO estudianteDTO);
        Task<ActionResponse<ListEstDTO>> GetEstudiante(string nroCi);
        Task<ActionResponse<ListEstDTO>> LoginEstAsync(LoginEstDTO model);
        //Task<ActionResponse<bool>> UpdateAsync(EstudianteDTO estudianteDTO);
        // Task<ActionResponse<ListEstDTO>> LoginEstAsync(LoginEstDTO model);
        Task<ActionResponse<IEnumerable<ListEstDTO>>> GetListAsync();
    }
}

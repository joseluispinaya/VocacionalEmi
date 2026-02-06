using Capa.Shared.DTOs;
using Capa.Shared.Responses;

namespace Capa.Web.Repositories.Intefaces
{
    public interface IUnidadesEduRepository
    {
        Task<ActionResponse<IEnumerable<ListUnidadEduDTO>>> GetListAsync();
    }
}

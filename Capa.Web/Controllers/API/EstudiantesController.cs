using Capa.Web.Repositories.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Capa.Web.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstudiantesController : ControllerBase
    {
        private readonly IEstudiantesRepository _estudiantesRepository;
        public EstudiantesController(IEstudiantesRepository estudiantesRepository)
        {
            _estudiantesRepository = estudiantesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var result = await _estudiantesRepository.GetListAsync();


            return Ok(result);
        }

    }
}

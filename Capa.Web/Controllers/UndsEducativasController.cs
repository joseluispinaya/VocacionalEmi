using Capa.Web.Repositories.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Capa.Web.Controllers
{
    public class UndsEducativasController : Controller
    {
        private readonly IUnidadesEduRepository _unidadesEduRepository;
        public UndsEducativasController(IUnidadesEduRepository unidadesEduRepository)
        {
            _unidadesEduRepository = unidadesEduRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaUnidadesEd()
        {
            var response = await _unidadesEduRepository.GetListAsync();

            // Retornamos todo el objeto response (WasSuccess, Message, Result)
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}

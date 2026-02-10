using Capa.Shared.DTOs;
using Capa.Shared.Responses;
using Capa.Web.Repositories.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Capa.Web.Controllers
{
    public class PreguntasController : Controller
    {
        private readonly IPreguntasRepository _preguntasRepository;
        public PreguntasController(IPreguntasRepository preguntasRepository)
        {
            _preguntasRepository = preguntasRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] PreguntaItemDTO objeto)
        {
            // Validación automática de Data Annotations ([Required], [MaxLength], etc.)
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status200OK, new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "Faltan campos obligatorios o son incorrectos."
                });
            }
            var response = await _preguntasRepository.AddAsync(objeto);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] PreguntaItemDTO objeto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status200OK, new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "Faltan campos obligatorios o son incorrectos."
                });
            }
            var response = await _preguntasRepository.UpdateAsync(objeto);

            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}

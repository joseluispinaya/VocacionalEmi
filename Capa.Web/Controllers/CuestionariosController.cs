using Capa.Shared.DTOs;
using Capa.Shared.Responses;
using Capa.Web.Repositories.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Capa.Web.Controllers
{
    public class CuestionariosController : Controller
    {
        private readonly ICuestionariosRepository _cuestionariosRepository;
        private readonly IPreguntasRepository _preguntasRepository;
        public CuestionariosController(ICuestionariosRepository cuestionariosRepository, IPreguntasRepository preguntasRepository)
        {
            _cuestionariosRepository = cuestionariosRepository;
            _preguntasRepository = preguntasRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaCuestionarios()
        {
            var response = await _cuestionariosRepository.GetListAsync();

            // Retornamos todo el objeto response (WasSuccess, Message, Result)
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] CuestionarioDTO objeto)
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
            var response = await _cuestionariosRepository.AddAsync(objeto);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] CuestionarioDTO objeto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status200OK, new ActionResponse<bool>
                {
                    WasSuccess = false,
                    Message = "Faltan campos obligatorios o son incorrectos."
                });
            }
            var response = await _cuestionariosRepository.UpdateAsync(objeto);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpGet]
        public async Task<IActionResult> DetalleCuestionario(int cuestionarioId)
        {
            // Esto llama a tu repositorio
            var response = await _preguntasRepository.GetDetalleAsync(cuestionarioId);

            // Retorna el objeto ActionResponse completo
            return StatusCode(StatusCodes.Status200OK, response);
        }
    }
}

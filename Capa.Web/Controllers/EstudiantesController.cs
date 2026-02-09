using Capa.Shared.DTOs;
using Capa.Shared.Responses;
using Capa.Web.Models;
using Capa.Web.Repositories.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace Capa.Web.Controllers
{
    public class EstudiantesController : Controller
    {
        private readonly IEstudiantesRepository _estudiantesRepository;
        public EstudiantesController(IEstudiantesRepository estudiantesRepository)
        {
            _estudiantesRepository = estudiantesRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaEstudia()
        {
            var response = await _estudiantesRepository.GetListAsync();
            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromForm] EstudianteDTO estudianteDTO)
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

            // Aquí estudianteDTO YA TIENE los datos (Nombres, NroCi) y el archivo (PhotoFile)
            // No necesitas deserializar nada.
            var response = await _estudiantesRepository.AddAsync(estudianteDTO);

            return StatusCode(StatusCodes.Status200OK, response);
        }

        [HttpPost]
        public async Task<IActionResult> Editar([FromForm] EstudianteDTO estudianteDTO)
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

            // Aquí estudianteDTO YA TIENE los datos (Nombres, NroCi) y el archivo (PhotoFile)
            // No necesitas deserializar nada.
            var response = await _estudiantesRepository.UpdateAsync(estudianteDTO);

            return StatusCode(StatusCodes.Status200OK, response);
        }

    }
}

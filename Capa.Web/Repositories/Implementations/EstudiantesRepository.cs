using Capa.Shared.DTOs;
using Capa.Shared.Entities;
using Capa.Shared.Responses;
using Capa.Web.Data;
using Capa.Web.Helpers;
using Capa.Web.Models;
using Capa.Web.Repositories.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace Capa.Web.Repositories.Implementations
{
    public class EstudiantesRepository : IEstudiantesRepository
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;
        public EstudiantesRepository(DataContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }

        public async Task<ActionResponse<bool>> AddAsync(EstudianteDTO estudianteDTO)
        {
            string? photoPath = null;

            try
            {
                var unidadEdu = await _context.UnidadEducativas.FindAsync(estudianteDTO.UnidadEducativaId);
                if (unidadEdu == null)
                {
                    return new ActionResponse<bool> { WasSuccess = false, Message = "Unidad educativa no encontrada." };
                }

                if (estudianteDTO.PhotoFile != null)
                    photoPath = await _fileStorage.UploadFileAsync(estudianteDTO.PhotoFile, "estudiante");

                var estudiante = new Estudiante
                {
                    NroCi = estudianteDTO.NroCi,
                    Nombres = estudianteDTO.Nombres,
                    Apellidos = estudianteDTO.Apellidos,
                    Correo = estudianteDTO.Correo,
                    Clave = estudianteDTO.NroCi,
                    Photo = photoPath,
                    UnidadEducativaId = estudianteDTO.UnidadEducativaId
                };

                _context.Add(estudiante);
                await _context.SaveChangesAsync();

                return new ActionResponse<bool> { WasSuccess = true, Result = true, Message = "Estudiante registrado correctamente." };
            }
            catch (DbUpdateException)
            {
                if (photoPath != null)
                    await _fileStorage.RemoveFileAsync(photoPath, "estudiante");

                return new ActionResponse<bool> { WasSuccess = false, Message = "Ya existe un estudiante con el Nro CI." };
            }
            catch
            {
                return new ActionResponse<bool> { WasSuccess = false, Message = "Ocurrió un error al registrar intente mas tarde." };
            }
        }

        public async Task<ActionResponse<IEnumerable<ListEstDTO>>> GetListAsync()
        {
            var estudiantes = await _context.Estudiantes.Include(x => x.UnidadEducativa)
                .Select(x => new ListEstDTO
                {
                    Id = x.Id,
                    NroCi = x.NroCi,
                    Nombres = x.Nombres,
                    Apellidos = x.Apellidos,
                    Correo = x.Correo,
                    Photo = x.Photo,
                    UnidadEducativaId = x.UnidadEducativaId,
                    UnidadEducativa = x.UnidadEducativa.Nombre
                }).ToListAsync();

            if (estudiantes.Count == 0)
            {
                return new ActionResponse<IEnumerable<ListEstDTO>>
                {
                    WasSuccess = false,
                    Message = "No hay Datos para mostrar."
                };
            }

            return new ActionResponse<IEnumerable<ListEstDTO>>
            {
                WasSuccess = true,
                Message = "Lista obtenida correctamente.",
                Result = estudiantes
            };
        }
    }
}

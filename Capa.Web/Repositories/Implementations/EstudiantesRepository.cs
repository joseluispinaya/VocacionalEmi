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

        public async Task<ActionResponse<bool>> UpdateAsync(EstudianteDTO estudianteDTO)
        {
            // 1. Buscar la entidad (EF Core ya la empieza a rastrear aquí)
            var estModel = await _context.Estudiantes.FindAsync(estudianteDTO.Id);
            if (estModel == null)
            {
                return new ActionResponse<bool> { WasSuccess = false, Message = "Registro no encontrado." };
            }

            string? rutaFotoVieja = estModel.Photo;
            string? rutaFotoNueva = null;

            // 2. Manejo de la NUEVA foto (Subida preliminar)
            if (estudianteDTO.PhotoFile != null)
            {
                rutaFotoNueva = await _fileStorage.UploadFileAsync(estudianteDTO.PhotoFile, "estudiante");
                // Asignamos la nueva ruta al modelo
                estModel.Photo = rutaFotoNueva;
            }

            // 3. Actualizar el resto de propiedades
            estModel.NroCi = estudianteDTO.NroCi;
            estModel.Nombres = estudianteDTO.Nombres;
            estModel.Apellidos = estudianteDTO.Apellidos;
            estModel.Correo = estudianteDTO.Correo;
            estModel.UnidadEducativaId = estudianteDTO.UnidadEducativaId;

            // No es necesario _context.Update(estModel);

            try
            {
                // 4. Intentar guardar en BD
                await _context.SaveChangesAsync();

                // ÉXITO: Si llegamos aquí, la BD ya se actualizó.
                // Ahora sí es seguro borrar la foto VIEJA (si es que subimos una nueva y había una vieja)
                if (rutaFotoNueva != null && !string.IsNullOrWhiteSpace(rutaFotoVieja))
                {
                    await _fileStorage.RemoveFileAsync(rutaFotoVieja, "estudiante");
                }

                return new ActionResponse<bool> { WasSuccess = true, Result = true, Message = "Estudiante actualizado correctamente." };
            }
            catch (DbUpdateException)
            {
                // ERROR DE BD: (Ej. CI duplicado)
                // Rollback del archivo: Borramos la foto NUEVA que acabamos de subir para no dejar basura.
                if (rutaFotoNueva != null)
                {
                    await _fileStorage.RemoveFileAsync(rutaFotoNueva, "estudiante");
                }

                return new ActionResponse<bool> { WasSuccess = false, Message = "Ya existe un estudiante con el Nro CI." };
            }
            catch (Exception)
            {
                // ERROR GENERAL
                // Rollback del archivo también aquí
                if (rutaFotoNueva != null)
                {
                    await _fileStorage.RemoveFileAsync(rutaFotoNueva, "estudiante");
                }
                return new ActionResponse<bool> { WasSuccess = false, Message = "Ocurrió un error al actualizar, intente más tarde." };
            }
        }
    }
}

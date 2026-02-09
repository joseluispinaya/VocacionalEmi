using Capa.Shared.DTOs;
using Capa.Shared.Entities;
using Capa.Shared.Responses;
using Capa.Web.Data;
using Capa.Web.Repositories.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace Capa.Web.Repositories.Implementations
{
    public class CuestionariosRepository : ICuestionariosRepository
    {
        private readonly DataContext _context;
        public CuestionariosRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<ActionResponse<bool>> AddAsync(CuestionarioDTO cuestionarioDTO)
        {
            try
            {
                var cuestionario = new Cuestionario
                {
                    Titulo = cuestionarioDTO.Titulo,
                    Descripcion = cuestionarioDTO.Descripcion,
                    FechaCreado = DateTime.UtcNow
                };
                _context.Add(cuestionario);
                await _context.SaveChangesAsync();

                return new ActionResponse<bool> { WasSuccess = true, Result = true, Message = "Cuestionario registrado correctamente." };
            }
            catch
            {
                return new ActionResponse<bool> { WasSuccess = false, Message = "Ocurrio un problema intente mas tarde." };
            }
        }

        public async Task<ActionResponse<IEnumerable<CuestionarioDTO>>> GetListAsync()
        {
            var cuestionarios = await _context.Cuestionarios
                .Select(c => new CuestionarioDTO
                {
                    Id = c.Id,
                    Titulo = c.Titulo,
                    Descripcion = c.Descripcion
                })
                .ToListAsync();

            if (cuestionarios.Count == 0)
            {
                return new ActionResponse<IEnumerable<CuestionarioDTO>>
                {
                    WasSuccess = false,
                    Message = "No se encontraron cuestionarios."
                };
            }

            return new ActionResponse<IEnumerable<CuestionarioDTO>>
            {
                WasSuccess = true,
                Message = "Lista obtenida correctamente.",
                Result = cuestionarios
            };
        }

        public async Task<ActionResponse<bool>> UpdateAsync(CuestionarioDTO cuestionarioDTO)
        {
            try // <--- Mueve el try al inicio
            {
                var cuestModel = await _context.Cuestionarios.FindAsync(cuestionarioDTO.Id);
                if (cuestModel == null)
                {
                    return new ActionResponse<bool> { WasSuccess = false, Message = "Registro no encontrado." };
                }

                cuestModel.Titulo = cuestionarioDTO.Titulo;
                cuestModel.Descripcion = cuestionarioDTO.Descripcion;

                await _context.SaveChangesAsync(); // EF Core detecta cambios automáticamente

                return new ActionResponse<bool> { WasSuccess = true, Result = true, Message = "Cuestionario actualizado correctamente." };
            }
            catch (Exception)
            {
                // Opcional: Loguear el error real 'ex' aquí
                return new ActionResponse<bool> { WasSuccess = false, Message = "Ocurrió un error al actualizar." };
            }
        }
    }
}

using GestionTareasAPI.Domain.Entities;

namespace GestionTareasAPI.Domain.Repositories
{
    public interface ITareasService
    {
        Task<Tarea> CrearAsync(Tarea nueva);
        Task CambiarEstadoAsync(int tareaId, EstadoTarea nuevoEstado);
        Task AsignarUsuarioAsync(int tareaId, int usuarioId);
        Task EliminarAsync(int tareaId);
    }
}

using GestionTareasAPI.Domain.Entities;

namespace GestionTareasAPI.Domain.Repositories
{
    public interface ITareasRepository
    {
        Task<Tarea?> GetByIdAsync(int id);
        Task<Tarea> AddAsync(Tarea tarea);
        Task RemoveAsync(Tarea tarea);
        Task<int> CountPendientesByUsuarioAsync(int usuarioId);
        Task SaveChangesAsync();
    }
}

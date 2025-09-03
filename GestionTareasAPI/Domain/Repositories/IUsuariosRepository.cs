using GestionTareasAPI.Domain.Entities;

namespace GestionTareasAPI.Domain.Repositories
{
    public interface IUsuariosRepository
    {
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario> AddAsync(Usuario usuario);
        Task<bool> EmailExistsAsync(string email);
    }
}

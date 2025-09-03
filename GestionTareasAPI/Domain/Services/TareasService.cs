using GestionTareasAPI.Domain.Common;
using GestionTareasAPI.Domain.Entities;
using GestionTareasAPI.Domain.Repositories;

namespace GestionTareasAPI.Domain.Services;

public class TareasService : ITareasService
{
    private readonly ITareasRepository _tareasRepo;
    private readonly IUsuariosRepository _usuariosRepo;

    public TareasService(ITareasRepository tareasRepo, IUsuariosRepository usuariosRepo)
        => (_tareasRepo, _usuariosRepo) = (tareasRepo, usuariosRepo);

    public async Task<Tarea> CrearAsync(Tarea t)
    {
        var usuario = await _usuariosRepo.GetByIdAsync(t.UsuarioId)
                     ?? throw new NotFoundException($"Usuario {t.UsuarioId} no existe");

        if (t.FechaVencimiento < t.FechaCreacion)
            throw new DomainException("La fecha de vencimiento no puede ser menor a la de creación");

        var pendientes = await _tareasRepo.CountPendientesByUsuarioAsync(usuario.Id);
        if (pendientes >= 5)
            throw new DomainException("El usuario ya tiene 5 tareas pendientes");

        t.Estado = EstadoTarea.Pendiente;

        var creada = await _tareasRepo.AddAsync(t);
        await _tareasRepo.SaveChangesAsync();
        return creada;
    }

    public async Task CambiarEstadoAsync(int tareaId, EstadoTarea nuevo)
    {
        var t = await _tareasRepo.GetByIdAsync(tareaId)
              ?? throw new NotFoundException($"Tarea {tareaId} no existe");

        if (nuevo == EstadoTarea.Completada && DateTime.UtcNow < t.FechaVencimiento)
            throw new DomainException("No se puede completar antes de la fecha de vencimiento");

        t.Estado = nuevo;
        await _tareasRepo.SaveChangesAsync();
    }

    public async Task AsignarUsuarioAsync(int tareaId, int usuarioId)
    {
        var t = await _tareasRepo.GetByIdAsync(tareaId)
              ?? throw new NotFoundException($"Tarea {tareaId} no existe");

        var u = await _usuariosRepo.GetByIdAsync(usuarioId)
              ?? throw new NotFoundException($"Usuario {usuarioId} no existe");

        t.UsuarioId = u.Id;
        await _tareasRepo.SaveChangesAsync();
    }

    public async Task EliminarAsync(int tareaId)
    {
        var t = await _tareasRepo.GetByIdAsync(tareaId)
              ?? throw new NotFoundException($"Tarea {tareaId} no existe");

        await _tareasRepo.RemoveAsync(t);
        await _tareasRepo.SaveChangesAsync();
    }
}
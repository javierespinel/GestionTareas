using GestionTareasAPI.Domain.Entities;

namespace GestionTareasAPI.Application.DTOs;

public record CreateTareaDto(
    string Titulo,
    string? Descripcion,
    DateTime FechaVencimiento,
    int UsuarioId
);

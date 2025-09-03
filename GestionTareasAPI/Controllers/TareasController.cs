using GestionTareasAPI.Infrastructure.Data;
using GestionTareasAPI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionTareasAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TareasController : ControllerBase
{
    private readonly AppDbContext _db;
    public TareasController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<object>> Get(
        [FromQuery] string? search, [FromQuery] EstadoTarea? estado,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var q = _db.Tareas.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(t => t.Titulo.Contains(search) || (t.Descripcion ?? "").Contains(search));
        if (estado.HasValue) q = q.Where(t => t.Estado == estado);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(t => t.Id)
                           .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Tarea>> GetById(int id)
        => await _db.Tareas.FindAsync(id) is { } t ? Ok(t) : NotFound();

    public record CreateTareaDto(string Titulo, string? Descripcion, DateTime FechaVencimiento, int UsuarioId);
    public record CambiarEstadoDto(EstadoTarea Estado);

    [HttpPost]
    public async Task<ActionResult<Tarea>> Post(CreateTareaDto dto)
    {
        if (await _db.Usuarios.FindAsync(dto.UsuarioId) is null)
            return NotFound($"Usuario {dto.UsuarioId} no existe");

        if (dto.FechaVencimiento < DateTime.UtcNow)
            return BadRequest("La fecha de vencimiento no puede ser menor a ahora.");

        var pendientes = await _db.Tareas.CountAsync(t => t.UsuarioId == dto.UsuarioId && t.Estado == EstadoTarea.Pendiente);
        if (pendientes >= 5) return BadRequest("El usuario ya tiene 5 tareas pendientes.");

        var t = new Tarea
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            FechaCreacion = DateTime.UtcNow,
            FechaVencimiento = dto.FechaVencimiento,
            Estado = EstadoTarea.Pendiente,
            UsuarioId = dto.UsuarioId
        };

        _db.Tareas.Add(t);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = t.Id }, t);
    }

    [HttpPut("{id:int}/estado")]
    public async Task<IActionResult> CambiarEstado(int id, CambiarEstadoDto body)
    {
        var t = await _db.Tareas.FindAsync(id);
        if (t is null) return NotFound();
        if (body.Estado == EstadoTarea.Completada && DateTime.UtcNow < t.FechaVencimiento)
            return BadRequest("No se puede completar antes de la fecha de vencimiento.");
        t.Estado = body.Estado;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Tareas.FindAsync(id);
        if (t is null) return NotFound();
        _db.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

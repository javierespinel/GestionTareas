using GestionTareasAPI.Infrastructure.Data;
using GestionTareasAPI.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionTareasAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsuariosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Usuario>>> Get()
        => Ok(await _db.Usuarios.AsNoTracking().ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Usuario>> GetById(int id)
        => await _db.Usuarios.FindAsync(id) is { } u ? Ok(u) : NotFound();

    public record CreateUsuarioDto(string Nombre, string Email);

    [HttpPost]
    public async Task<ActionResult<Usuario>> Post(CreateUsuarioDto dto)
    {
        if (await _db.Usuarios.AnyAsync(x => x.Email == dto.Email))
            return Conflict("El email ya está registrado.");

        var u = new Usuario { Nombre = dto.Nombre, Email = dto.Email };
        _db.Usuarios.Add(u);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = u.Id }, u);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var u = await _db.Usuarios.FindAsync(id);
        if (u is null) return NotFound();
        _db.Remove(u);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

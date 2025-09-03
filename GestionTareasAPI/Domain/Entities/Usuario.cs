using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;


namespace GestionTareasAPI.Domain.Entities

{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Email { get; set; } = null!;

        public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    }
}

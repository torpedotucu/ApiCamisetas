using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiCamisetas.Models
{
    public class CamisetaCreateDTO
    {

        public string Equipo { get; set; }
        public string CodigoPais { get; set; }
        public int Year { get; set; }
        public string Marca { get; set; }
        public string Equipacion { get; set; }
        public int? Posicion { get; set; }
        public string Condicion { get; set; }
        public int? Dorsal { get; set; } // Puede ser nulo si no todas las camisetas tienen número
        public string? Jugador { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen { get; set; }
    }
}

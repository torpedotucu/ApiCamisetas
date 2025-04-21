namespace ApiCamisetas.Models
{
    public class CamisetaUpdateDTO
    {
        public int IdCamiseta { get; set; }
        public int? Year { get; set; }
        public string? Marca { get; set; }
        public string Condicion { get; set; }
        public int? Dorsal { get; set; } // Puede ser nulo si no todas las camisetas tienen número
        public string? Jugador { get; set; }
        public string? Descripcion { get; set; }
        public string? Imagen { get; set; }
    }
}

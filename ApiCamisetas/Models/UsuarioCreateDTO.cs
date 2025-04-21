using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiCamisetas.Models
{
    public class UsuarioCreateDTO
    {       
        public string UserName { get; set; }
        public string AliasName { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public string Avatar { get; set; } 
        public string Equipo { get; set; }
        public string Pais { get; set; }
    }
}

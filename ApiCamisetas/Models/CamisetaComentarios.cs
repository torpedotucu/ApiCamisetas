using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ApiCamisetas.Models;

namespace ApiCamisetas.Models
{
    
    public class CamisetaComentarios
    {

        public Camiseta Camiseta { get; set; }

        public List<Comentario> Comentarios { get; set; }

    }
}

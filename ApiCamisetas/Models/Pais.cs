using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiCamisetas.Models
{
    [Table("paises")]
    public class Pais
    {
        [Key]
        [Column("codigo")]
        public string CodigoPais { get; set; }
        [Column("nombre")]
        public string NombrePais { get; set; }

    }
}

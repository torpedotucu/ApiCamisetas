using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCamisetas.Models
{
    [Table("LIKES")]
    public class Like
    {
        [Key]
        [Column("LIKEID")]
        public int idLike { get; set; }
        [Column("CAMISETAID")]
        public int idCamiseta { get; set; }
        [Column("USUARIOID")]
        public int idUsuario { get; set; }
        [Column("FECHALIKE")]
        public DateTime FechaLike { get; set; }

    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiCamisetas.Models
{        
    [Table("COMENTARIOS")]
    public class ComentarioDTO
    {
        //[Key]
        //[Column("IDCOMENTARIO")]
        //public int IdComentario { get; set; }
        [Column("CAMISETAID")]
        public int CamisetaId { get; set; }
        [Column("USUARIOID")]
        public int UsuarioId { get; set; }
        [Column("TEXTOCOMENTARIO")]
        public string ComentarioTxt { get; set; }
        //[Column("FECHACOMENTARIO")]
        //public DateTime FechaComentario { get; set; }
    }  
}

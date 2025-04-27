using ApiCamisetas.Models;
using Microsoft.EntityFrameworkCore;
using NugetJerseyHubRGO.Models;

namespace ApiCamisetas.Data
{
    public class CamisetasContext: DbContext
    {
        public CamisetasContext(DbContextOptions<CamisetasContext> options) : base(options)
        {
        }

        public DbSet<Camiseta> Camisetas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        //public DbSet<ComentarioDTO> ComentarioDTOs { get; set; }
        public DbSet<UsuarioPuro> UsuariosPuros { get; set; }
        public DbSet<Pais> Paises { get; set; }

        public DbSet<Like> Likes { get; set; }

        public DbSet<Amistad> Amistades { get; set; }

        public DbSet<Etiqueta> Etiquetas { get; set; }

    }
}

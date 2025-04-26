using ApiCamisetas.Data;
using ApiCamisetas.Helpers;
using ApiCamisetas.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiCamisetas.Repositories
{
    public class RepositoryCamisetas
    {
        private CamisetasContext context;
        private IWebHostEnvironment hostEnvironment;
        private HelperPathProvider helperPathProvider;

        public RepositoryCamisetas(CamisetasContext context, IWebHostEnvironment hostEnvironment, HelperPathProvider helperPathProvider)
        {
            this.context=context;
            this.hostEnvironment=hostEnvironment;
            this.helperPathProvider=helperPathProvider;
        }

        public async Task<Usuario?>LoginUsuario(string email,string contrasena)
        {
            var consulta = this.context.UsuariosPuros.Where(x => x.Correo==email);
            UsuarioPuro user = await consulta.FirstOrDefaultAsync();
            if (user==null)
            {
                return null;
            }
            else
            {
                string salt = user.Salt;
                byte[] temp = HelperCryptography.EncryptPassword(contrasena, salt);
                byte[] passBytes = user.Contrasena;
                bool response = HelperCryptography.CompararArrays(temp, passBytes);
                if (response==true)
                {
                    Usuario usuario = await this.context.Usuarios.Where(u => u.IdUsuario==user.IdUsuario).FirstOrDefaultAsync();
                    return usuario;
                }
                else
                {
                    return null;
                }
            }
        }
        public async Task<UsuarioPuro>GetUsuario(int idUsuario)
        {
            //var consulta = from datos in this.context.UsuariosPuros
            //               where datos.IdUsuario==idUsuario
            //               select datos;
            //return await this.context.UsuariosPuros.FirstOrDefaultAsync();
            return await this.context.UsuariosPuros
                .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
        }
        public async Task<Usuario> GetUsuarioLibre(int idUsuario)
        {
            var consulta = await this.context.Usuarios
                .Where(x => x.IdUsuario==idUsuario).FirstOrDefaultAsync();
            return consulta;
        }

        public async Task<List<Camiseta>> GetCamisetasUsuario(int idUsuario)
        {

            var consulta = this.context.Camisetas.Where(c => c.IdUsuario==idUsuario);

            List<Camiseta> camisetas = await consulta.ToListAsync();

            return camisetas;
        }
        public async Task<Camiseta> GetCamiseta(int idCamiseta)
        {
            //var consulta = from datos in this.context.Camisetas
            //               where datos.IdCamiseta == idCamiseta
            //               select datos;
            var consulta = this.context.Camisetas.Include(p => p.Pais)
                .Where(c => c.IdCamiseta==idCamiseta);
            Camiseta camiseta = await consulta.FirstOrDefaultAsync();
            return camiseta;
        }

        public async Task<int> SubirCamiseta(int idUsuario,CamisetaCreateDTO camiseta)
        { 
            Camiseta camiseta1 = new Camiseta();
            camiseta1.Equipo = camiseta.Equipo;
            camiseta1.IdCamiseta=await this.GetMaxIdCamiseta();
            camiseta1.CodigoPais = camiseta.CodigoPais;
            camiseta1.Year = camiseta.Year;
            camiseta1.Marca = camiseta.Marca;
            camiseta1.Equipacion = camiseta.Equipacion;
            camiseta1.Condicion = camiseta.Condicion;
            camiseta1.Dorsal = camiseta.Dorsal;
            camiseta1.Jugador = camiseta.Jugador;
            camiseta1.EsActiva = 1;
            camiseta1.IdUsuario = idUsuario;
            camiseta1.FechaSubida = DateTime.Now;
            await this.context.Camisetas.AddAsync(camiseta1);
            await this.context.SaveChangesAsync();
            return camiseta1.IdCamiseta;
        }

        public async Task ModificarCamiseta(int idCamiseta,CamisetaUpdateDTO camiseta)
        {
            Camiseta cam = await this.GetCamiseta(idCamiseta);

            if (cam == null)
            {
                throw new Exception("Camiseta no encontrada.");
            }

            bool cambios = false;


            if (camiseta.Year.HasValue && cam.Year != camiseta.Year.Value)
            {
                cam.Year = camiseta.Year.Value;
                cambios = true;
            }

            if (!string.IsNullOrEmpty(camiseta.Marca) && cam.Marca != camiseta.Marca)
            {
                cam.Marca =camiseta.Marca;
                cambios = true;
            }


            if (!string.IsNullOrEmpty(camiseta.Descripcion) && cam.Descripcion != camiseta.Descripcion)
            {
                cam.Descripcion = camiseta.Descripcion;
                cambios = true;
            }

            if (!string.IsNullOrEmpty(camiseta.Condicion) && cam.Condicion != camiseta.Condicion)
            {
                cam.Condicion = camiseta.Condicion;
                cambios = true;
            }

            if (camiseta.Dorsal.HasValue && cam.Dorsal != camiseta.Dorsal.Value)
            {
                cam.Dorsal = camiseta.Dorsal.Value;
                cambios = true;
            }

            if (!string.IsNullOrEmpty(camiseta.Jugador) && cam.Jugador != camiseta.Jugador)
            {
                cam.Jugador = camiseta.Jugador;
                cambios = true;
            }
            if (!string.IsNullOrEmpty(camiseta.Imagen) && cam.Imagen != camiseta.Imagen)
            {
                cam.Imagen = camiseta.Imagen;
                cambios = true;
            }

            // Guardar cambios solo si hubo modificaciones
            if (cambios)
            {
                this.context.Camisetas.Update(cam);
                await this.context.SaveChangesAsync();
            }
        }


        //REVISAR
        public async Task EditarPerfil(int idUsuario,UsuarioUpdateDTO usuario)
        {
            UsuarioPuro usuarioPuro = await this.GetUsuario(idUsuario);
            if (usuarioPuro == null)
            {
                throw new Exception("Usuario no encontrado.");
            }
            bool cambios = false;
            if (usuario.Alias!=null)
            {
                usuarioPuro.AliasName=usuario.Alias;
                cambios=true;
            }
            if (usuario.Equipo!=null)
            {
                usuarioPuro.Equipo=usuario.Equipo;
                cambios=true;
            }
            //if (avatar!=null)
            //{
            //    string filename = this.GenerateUniqueFileName(idUser, avatar);
            //    usuarioPuro.Avatar=filename;
            //    await this.SubirFichero(avatar, Folders.Avatar, filename);
            //    cambios=true;
            //}
            if (usuario.Contrasena!=null)
            {
                usuarioPuro.Salt=HelperCryptography.GenerateSalt();
                usuarioPuro.Contrasena=HelperCryptography.EncryptPassword(usuario.Contrasena, usuarioPuro.Salt);
                cambios=true;
            }
            if (cambios)
            {
                this.context.UsuariosPuros.Update(usuarioPuro);
                await this.context.SaveChangesAsync();
            }


        }
        public async Task<List<Comentario>> GetComentariosAsync(int idCamiseta)
        {
            var comentarios = await this.context.Comentarios.Include(c => c.Usuario).Where(c => c.CamisetaId==idCamiseta).ToListAsync();
            return comentarios;
        }
        public async Task<CamisetaComentarios> DetalleCamiseta(int idCamiseta)
        {

            Camiseta camiseta = await this.GetCamiseta(idCamiseta);
            List<Comentario> comentarios = await this.GetComentariosAsync(idCamiseta);
            CamisetaComentarios camisetaComentarios = new CamisetaComentarios();
            camisetaComentarios.Camiseta=camiseta;
            camisetaComentarios.Comentarios=comentarios;
            return camisetaComentarios;
        }

        public async Task Comentar(ComentarioDTO com)
        {
            Comentario comentario = new Comentario() ;
            comentario.IdComentario= await this.GetMaxIdComment();
            comentario.UsuarioId=com.UsuarioId;
            comentario.CamisetaId=com.CamisetaId;
            comentario.ComentarioTxt=com.ComentarioTxt;
            comentario.Usuario=await this.GetUsuarioLibre(com.UsuarioId);
            comentario.FechaComentario=DateTime.UtcNow;
            await this.context.Comentarios.AddAsync(comentario);
            await this.context.SaveChangesAsync();
        }


        public async Task<int> GetMaxIdCamiseta()
        {
            if (this.context.Camisetas.Count()==0)
            {
                return 1;
            }
            else
            {
                return await this.context.Camisetas.MaxAsync(x => x.IdCamiseta)+1;
            }
        }

        public async Task<int> GetMaxIdUsuario()
        {
            if (this.context.UsuariosPuros.Count()==0)
            {
                return 1;
            }
            else
            {
                int id = await this.context.UsuariosPuros.MaxAsync(x => x.IdUsuario)+1;
                return id;
            }
        }

        public async Task<int> GetMaxIdComment()
        {
            if (this.context.Comentarios.Count()==0)
            {
                return 1;
            }
            else
            {
                return await this.context.Comentarios.MaxAsync(x => x.IdComentario)+1;
            }
        }

        public async Task<int> GetMaxIdLike()
        {
            if (this.context.Likes.Count()==0)
            {
                return 1;
            }
            else
            {
                return await this.context.Likes.MaxAsync(x => x.idLike)+1;
            }
        }

        public async Task<List<Pais>> GetPaisesAsync()
        {
            var consulta = from datos in this.context.Paises
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<int> GetMaxIdAmistad()
        {
            if (this.context.Amistades.Count()==0)
            {
                return 1;
            }
            else
            {
                return await this.context.Amistades.MaxAsync(x => x.IdAmistad)+1;
            }
        }

        public async Task SubirFichero(IFormFile file, Folders folders, string nombreArchivo)
        {
            string rootFolder = this.hostEnvironment.WebRootPath;

            string path = this.helperPathProvider.MapPath(nombreArchivo, folders);

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }

        public async Task CreateUsuario(UsuarioCreateDTO usuario)
        {
            UsuarioPuro user=new UsuarioPuro();
            user.IdUsuario=await this.GetMaxIdUsuario();
            user.UserName=usuario.UserName;
            user.AliasName=usuario.AliasName;
            user.Correo=usuario.Correo.ToLower();
            string salt = HelperCryptography.GenerateSalt();
            user.Salt=salt;
            user.Contrasena=HelperCryptography.EncryptPassword(usuario.Contrasena, salt);
            user.Avatar=usuario.Avatar;
            user.Equipo=usuario.Equipo;
            user.Pais=usuario.Pais;
            user.CodeAmistad=this.GenerateCodeAmistadUsuario();
            user.FechaUnion=DateTime.UtcNow;
            //usuario.CodeAmistad=GenerateCodeAmistadUsuario();
            await this.context.UsuariosPuros.AddAsync(user);
            await this.context.SaveChangesAsync();
        }

        public string GenerateCodeAmistadUsuario()
        {
            return Guid.NewGuid().ToString().Substring(0, 9).ToUpper();

        }

        public async Task<Usuario> FindUsuarioAmistadCode(string friendCode)
        {
            var consulta = await this.context.Usuarios.Where(x => x.CodeAmistad==friendCode).FirstOrDefaultAsync();
            return consulta;
        }

        public async Task<bool> AreAlreadyFriends(int usuarioA, int UsuarioB)
        {
            return await this.context.Amistades.AnyAsync
                (a => (a.UsuarioId==usuarioA && a.AmigoId==UsuarioB) ||
                (a.UsuarioId==UsuarioB && a.AmigoId==usuarioA)
            );
        }

        public async Task SetAmistad(int userAId, int userBId)
        {
            var newAmistad = new Amistad
            {
                IdAmistad=await GetMaxIdAmistad(),
                UsuarioId=userAId,
                AmigoId=userBId,
                FechaAmistad=DateTime.UtcNow.Date
            };
            await this.context.Amistades.AddAsync(newAmistad);
            await this.context.SaveChangesAsync();
        }

        public async Task<int> GetNumberAmigosAsync(int idUsuario)
        {
            var consulta = await this.context.Amistades
                .Where(a => a.UsuarioId==idUsuario || a.AmigoId==idUsuario)
                .CountAsync();
            return consulta;
        }

        public async Task<List<Usuario>> GetListaAmigosAsync(int idUsuario)
        {
            var sql = "SP_OBTENER_AMIGOS_USUARIO @idusuario";
            SqlParameter paramIdUsuario = new SqlParameter("@idusuario", idUsuario);
            var consulta = this.context.Usuarios.FromSqlRaw(sql, paramIdUsuario);
            List<Usuario> amigos = await consulta.ToListAsync();
            return amigos;
        }


        //PARA VERSION DOS
        //COMPROBAR QUE EL USUARIO TIENE LIKE
        public async Task AddLike(int idUser, int idCamiseta)
        {
            Like like = new Like
            {
                idLike=await this.GetMaxIdLike(),
                idUsuario=idUser,
                idCamiseta=idCamiseta,
                FechaLike=DateTime.Now
            };
            await this.context.Likes.AddAsync(like);
            await this.context.SaveChangesAsync();
        }
        public async Task HadLike(int camisetaId, int userId)
        {
            var like = await this.context.Likes.FirstOrDefaultAsync(l => l.idUsuario==userId && l.idCamiseta==camisetaId);
            if (like==null)
            {
                this.AddLike(userId, camisetaId);
            }
            else
            {
                this.context.Likes.Remove(like);
                await this.context.SaveChangesAsync();
            }

        }

        public async Task InsertEtiquetas(List<string> etiquetas, int idCamiseta)
        {
            Console.Write(etiquetas);
            foreach (string etiqueta in etiquetas)
            {

                var sql = "SP_INSERT_ETIQUETA_CAMISETA @nombreEtiqueta, @idCamiseta";
                SqlParameter paramNombre = new SqlParameter("@nombreEtiqueta", etiqueta.ToLower());
                SqlParameter paramCamiseta = new SqlParameter("@idCamiseta", idCamiseta);
                var consulta = await this.context.Etiquetas.FromSqlRaw(sql, paramNombre, paramCamiseta).ToListAsync();


            }
        }

        public async Task<List<Etiqueta>> GetEtiquetas(int idCamiseta)
        {
            var sql = "SP_GET_ETIQUETA_CAMISETA @idCamiseta";
            SqlParameter paramCamiseta = new SqlParameter("@idCamiseta", idCamiseta);
            var consulta = await this.context.Etiquetas.FromSqlRaw(sql, paramCamiseta).ToListAsync();
            return consulta;
        }

        public string GenerateUniqueFileName(int idUser, IFormFile archivo)
        {
            string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
            string extension = Path.GetExtension(archivo.FileName);
            string nombreUnico = $"{idUser}_{timeStamp}{extension}";
            return nombreUnico;
        }

        public async Task<List<Camiseta>> GetPublicacionesInicio(int idUsuario)
        {
            var sql = "SP_GET_INICIO @idUsuario";
            SqlParameter paramUsuario = new SqlParameter("@idUsuario", idUsuario);
            List<Camiseta> consulta = await this.context.Camisetas.FromSqlRaw(sql, paramUsuario).ToListAsync();
            foreach (var camiseta in consulta)
            {
                camiseta.Usuario = await this.context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == camiseta.IdUsuario);
            }
            return consulta;
        }

        public async Task DeleteCamiseta(int idCamiseta)
        {
            Camiseta cam = await this.context.Camisetas.Where(c => c.IdCamiseta==idCamiseta).FirstOrDefaultAsync();
            this.context.Remove(cam);
            await this.context.SaveChangesAsync();
        }
        public async Task<bool> ExisteCorreo(string email)
        {
            return await this.context.Usuarios.AnyAsync(u => u.Correo == email.ToLower());
        }

        public async Task<bool>ExistePais(string codigoPais)
        {
            return await this.context.Paises.AnyAsync(u => u.CodigoPais==codigoPais);
        }


        public async Task<List<Comentario>> GetComentariosAsync()
        {
            return await this.context.Comentarios.ToListAsync();
        }

        public async Task<List<UsuarioPuro>> GetUsuarioPurosAsync()
        {
            return await this.context.UsuariosPuros.ToListAsync();
        }

        public async Task DeleteComentario(int idComentario)
        {
            Comentario comentario = await this.context.Comentarios.Where(x => x.IdComentario==idComentario).FirstOrDefaultAsync();
            this.context.Remove(comentario);
            await this.context.SaveChangesAsync();
        }
        public async Task<int> GetComentarioUsuario(int idComentario)
        {
            Comentario c  =await this.context.Comentarios.Where(x => x.IdComentario==idComentario).FirstOrDefaultAsync();
            return c.UsuarioId;
        }
    }
}

using ApiCamisetas.Helpers;
using ApiCamisetas.Models;
using ApiCamisetas.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ApiCamisetas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private RepositoryCamisetas repo;
        private HelperUsuarioToken helper;
        public UsuariosController(RepositoryCamisetas repo
            , HelperUsuarioToken helper)
        {
            this.repo = repo;
            this.helper = helper;
        }
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> PerfilUsuario()
        {
            Usuario usuario = this.helper.GetUsuario();

            if (usuario == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(usuario);
            }
        }
        [Authorize]
        [HttpGet]
        [Route("[action]/{idUsuario}")]
        public async Task<ActionResult> GetUsuario(int idUsuario)
        {
            Usuario usuario = await this.repo.GetUsuarioLibre(idUsuario);
            if (usuario==null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{correo}")]
        public async Task<ActionResult<Usuario>>UsuarioCorreo(string correo)
        {
            Usuario usuario = await this.repo.GetUsuarioCorreo(correo);
            if (usuario==null)
            {
                return NotFound();
            }
            return usuario;
        }

        [Authorize]
        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult> EditarPerfil([FromBody] UsuarioUpdateDTO user)
        {
            int idUsuario = this.helper.GetUsuario().IdUsuario;
            await this.repo.EditarPerfil(idUsuario, user);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> CreateUsuario([FromBody] UsuarioCreateDTO usuario)
        {
            if (await this.repo.ExisteCorreo(usuario.Correo.ToLower()))
            {
                return Conflict(new { mensaje = "El correo ya está en uso" });
            }
            if (!await this.repo.ExistePais(usuario.Pais))
            {
                return Conflict(new { mensaje = "El codigo de pais no existe" });
            }
            await this.repo.CreateUsuario(usuario);
            List<UsuarioPuro> usuarioPuros = await this.repo.GetUsuarioPurosAsync();
            return Ok(usuarioPuros);

        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> PublicacionesInicio()
        {
            Usuario user = this.helper.GetUsuario();
            List<Camiseta> camisetas = await this.repo.GetPublicacionesInicio(user.IdUsuario);

            return Ok(camisetas);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{codigoAmigo}")]
        public async Task<ActionResult> Amigo(string codigoAmigo)
        {
            Usuario usuario = await this.repo.FindUsuarioAmistadCode(codigoAmigo);
            if (usuario==null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> Amigos()
        {
            int idUsuario = this.helper.GetUsuario().IdUsuario;
            List<Usuario> amigos = await this.repo.GetListaAmigosAsync(idUsuario);
            return Ok(amigos);
        }


        [Authorize]
        [HttpPost]
        [Route("[action]/{idUsuario}")]
        public async Task<ActionResult> SetAmistad(int idUsuario)
        {
            int usuarioA = this.helper.GetUsuario().IdUsuario;
            int usuarioB = idUsuario;
            if (await this.repo.AreAlreadyFriends(usuarioA, usuarioB))
            {
                return Conflict(new { mensaje = "Ya son amigos " });
            }
            await this.repo.SetAmistad(usuarioA, usuarioB);
            return Ok();

        }

        

        
    }
}

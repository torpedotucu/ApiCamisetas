using ApiCamisetas.Helpers;
using ApiCamisetas.Models;
using ApiCamisetas.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCamisetas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private RepositoryCamisetas repo;
        private HelperUsuarioToken helper; 
        public ComentariosController(RepositoryCamisetas repo, HelperUsuarioToken helper)
        {
            this.helper=helper;
            this.repo=repo;
        }

        [HttpGet]
        [Route("[action]/{idCamiseta}")]
        public async Task<ActionResult> ComentariosCamiseta(int idCamiseta)
        {
            List<Comentario> comentarios = await this.repo.GetComentariosAsync(idCamiseta);
            return Ok(comentarios);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Comentar([FromBody] ComentarioDTO comentario)
        {
            comentario.UsuarioId= this.helper.GetUsuario().IdUsuario;

            await this.repo.Comentar(comentario);
            return Ok();
        }

        //PRUEBA
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> Comentarios()
        {
            List<Comentario> comentarios = await this.repo.GetComentariosAsync();
            return Ok(comentarios);
        }
        [Authorize]
        [HttpDelete]
        [Route("[action]/{idComentario}")]
        public async Task<ActionResult> DeleteComentario(int idComentario)
        {
            int idUsuarioComentario = await this.repo.GetComentarioUsuario(idComentario);
            int idToken = this.helper.GetUsuario().IdUsuario;
            if (idToken==idUsuarioComentario)
            {
                return Forbid();
            }
            await this.repo.DeleteComentario(idComentario);
            return Ok(new { response = "Comentario eliminado" });
        }
    }
}

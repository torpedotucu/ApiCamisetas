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

        [HttpGet]
        [Route("[action]/{idUsuario}")]
        public async Task<ActionResult>GetUsuario(int idUsuario)
        {
            Usuario usuario = await this.repo.GetUsuarioLibre(idUsuario);
            if (usuario==null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }
    }
}

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
    public class CamisetasController : ControllerBase
    {
        private RepositoryCamisetas repo;
        private HelperUsuarioToken helper;
        public CamisetasController(RepositoryCamisetas repo
            , HelperUsuarioToken helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult> CamisetasUsuario()
        {
            Usuario usuario = this.helper.GetUsuario();
            List<Camiseta> camisetas = await this.repo.GetCamisetasUsuario(usuario.IdUsuario);
            if (camisetas == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(camisetas);
            }
        }
        [Authorize]
        [HttpGet]
        [Route("[action]/{idCamiseta}")]
        public async Task<ActionResult> Camiseta(int idCamiseta)
        {
            Camiseta camiseta = await this.repo.GetCamiseta(idCamiseta);
            if (camiseta == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(camiseta);
            }
        }
        [Authorize]
        [HttpGet]
        [Route("DetallesCamiseta/{idcamiseta}")]
        public async Task<ActionResult> DetallesCamiseta(int idcamiseta)
        {
            CamisetaComentarios camiseta = await this.repo.DetalleCamiseta(idcamiseta);
            return Ok(camiseta);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> InsertarCamiseta([FromBody]CamisetaCreateDTO camiseta)
        {
            Usuario usuario = this.helper.GetUsuario();
            
            int idCamiseta = await this.repo.SubirCamiseta(usuario.IdUsuario,camiseta);
            if (idCamiseta == null || idCamiseta==0)
            {
                return BadRequest();
            }
            else
            {
                return Ok(idCamiseta);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("[action]/{idCamiseta}")]
        public async Task<ActionResult> EliminarCamiseta(int idCamiseta)
        {
            Usuario usuario = this.helper.GetUsuario();
            Camiseta camiseta = await this.repo.GetCamiseta(idCamiseta);

            if (camiseta == null)
            {
                return NotFound("Camiseta no encontrada.");
            }

            if (camiseta.IdUsuario != usuario.IdUsuario)
            {
                return Forbid(); 
            }

            await this.repo.DeleteCamiseta(idCamiseta);
            return Ok();

        }


        //CAMISETADTO ESPECIFICA PARA ACTUALIZAR? INCLUYENDO IDCAMISETA
        [Authorize]
        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult> ActualizarCamiseta([FromBody] CamisetaUpdateDTO camiseta)
        {
            Usuario usuario = this.helper.GetUsuario();
            Camiseta camiseta1 = await this.repo.GetCamiseta(camiseta.IdCamiseta);
            if (camiseta1 == null)
            {
                return NotFound("Camiseta no encontrada.");
            }
            if (camiseta1.IdUsuario != usuario.IdUsuario)
            {
                return Forbid();
            }
            await this.repo.ModificarCamiseta(camiseta.IdCamiseta, camiseta);
            return Ok();
        }


        

        [Authorize]
        [HttpPost]
        [Route("[action]/{idCamiseta}")]
        public async Task<ActionResult> InsertEtiquetas([FromBody]List<string>etiquetas,int idCamiseta)
        {
            await this.repo.InsertEtiquetas(etiquetas, idCamiseta);
            return Ok();
        }

        [Authorize]
        [HttpGet]
        [Route("[action]/{idcamiseta}")]
        public async Task<IActionResult>EtiquetasCamiseta(int idcamiseta)
        {
            List<Etiqueta> etiquetas = await this.repo.GetEtiquetas(idcamiseta);
            return Ok(etiquetas);
        }


    }
}

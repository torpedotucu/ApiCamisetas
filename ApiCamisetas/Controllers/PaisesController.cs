using ApiCamisetas.Models;
using ApiCamisetas.Repositories;
using Microsoft.AspNetCore.Mvc;
using NugetJerseyHubRGO.Models;


namespace ApiCamisetas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaisesController : ControllerBase
    {
        private RepositoryCamisetas repo;
        public PaisesController(RepositoryCamisetas repo)
        {
            this.repo=repo;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Pais>>> Paises()
        {
            List<Pais> paises = await this.repo.GetPaisesAsync();
            return paises;
        }

        [HttpGet]
        [Route("[action]/{codigoPais}")]
        public async Task<ActionResult<Pais>> Pais(string codigoPais)
        {
            Pais pais = await this.repo.GetPais(codigoPais);
            if (pais==null)
            {
                return NotFound();
            }
            return pais;
        }
    }
}

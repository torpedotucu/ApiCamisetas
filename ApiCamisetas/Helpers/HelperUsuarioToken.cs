using ApiCamisetas.Models;
using Newtonsoft.Json;
using NugetJerseyHubRGO.Models;
using System.Security.Claims;

namespace ApiCamisetas.Helpers
{
    public class HelperUsuarioToken
    {
        private IHttpContextAccessor contextAccessor;

        public HelperUsuarioToken(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor=contextAccessor;
        }
        public Usuario GetUsuario()
        {
            Claim claim = this.contextAccessor.HttpContext.User.FindFirst(x => x.Type=="UserData");
            string json = claim.Value;
            string jsonUsuario = HelperCryptography.DecryptString(json);
            Usuario usuario = JsonConvert.DeserializeObject<Usuario>(jsonUsuario);
            return usuario;
        }
    }
}

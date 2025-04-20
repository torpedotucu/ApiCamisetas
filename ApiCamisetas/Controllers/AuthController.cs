﻿using ApiCamisetas.Helpers;
using ApiCamisetas.Models;
using ApiCamisetas.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiCamisetas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryCamisetas repo;
        //CUANDO GENEREMOS EL TOKEN DEBEMOS INTEGRAR
        //ALGUNOS DATOS COMO ISSUER Y DEMAS
        private HelperActionServicesOAuth helper;

        public AuthController(RepositoryCamisetas repo
            , HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> 
            Login(LoginModel model)
        {
            Usuario user = await
                this.repo.LoginUsuario(model.UserName
                , model.Password);
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                //DEBEMOS CREAR UNAS CREDENCIALES PARA 
                //INCLUIRLAS DENTRO DEL TOKEN Y QUE ESTARAN 
                //COMPUESTAS POR EL SECRET KEY CIFRADO Y EL 
                //TIPO DE CIFRADO QUE INCLUIREMOS EN EL TOKEN
                SigningCredentials credentials =
                    new SigningCredentials
                    (this.helper.GetKeyToken(),
                    SecurityAlgorithms.HmacSha256);
                //CREAMOS EL OBJETO MODEL PARA ALMACENARLO 
                //DENTRO DEL TOKEN
                Usuario modelUsuario = new Usuario();
                modelUsuario.IdUsuario=user.IdUsuario;
                modelUsuario.UserName=user.UserName;
                modelUsuario.Alias=user.Alias;
                modelUsuario.Correo=user.Correo;
                modelUsuario.Avatar=user.Avatar;
                modelUsuario.Equipo=user.Equipo;
                modelUsuario.CodeAmistad=user.CodeAmistad;
                modelUsuario.Pais=user.Pais;
                modelUsuario.FechaUnion=user.FechaUnion;

                //CONVERTIMOS A JSON LOS DATOS DEL EMPLEADO
                string jsonUsuario =
                    JsonConvert.SerializeObject(modelUsuario);
                string jsonCrifado =
                    HelperCryptography.EncryptString(jsonUsuario);
                //CREAMOS UN ARRAY DE CLAIMS
                Claim[] informacion = new[]
                {
                    new Claim("UserData", jsonCrifado)
                };
                //EL TOKEN SE GENERA CON UNA CLASE
                //Y DEBEMOS INDICAR LOS DATOS QUE ALMACENARA EN SU 
                //INTERIOR
                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: informacion,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(20),
                        notBefore: DateTime.UtcNow
                        );
                //POR ULTIMO, DEVOLVEMOS LA RESPUESTA AFIRMATIVA
                //CON UN OBJETO QUE CONTENGA EL TOKEN (anonimo)
                return Ok(new
                {
                    response =
                    new JwtSecurityTokenHandler()
                    .WriteToken(token)
                });
            }
        }
    }
}

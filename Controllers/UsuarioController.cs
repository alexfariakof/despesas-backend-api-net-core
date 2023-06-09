﻿using despesas_backend_api_net_core.Business.Generic;
using despesas_backend_api_net_core.Business.Implementations;
using despesas_backend_api_net_core.Domain.Entities;
using despesas_backend_api_net_core.Domain.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlX.XDevAPI;
using System.Text.RegularExpressions;

namespace despesas_backend_api_net_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private IBusiness<UsuarioVM> _usuarioBusiness;
        private string bearerToken;
        public UsuarioController(IBusiness<UsuarioVM> usuarioBusiness)
        {
            _usuarioBusiness = usuarioBusiness;
        }

        [HttpGet("{IdUsuario}")]
        [Authorize("Bearer")]
        public IActionResult Get([FromRoute]int idUsuario)
        {
            bearerToken = HttpContext.Request.Headers["Authorization"].ToString();
            var _idUsuario = ControleAcessoBusinessImpl.getIdUsuarioFromToken(bearerToken);

            if (_idUsuario.Value != idUsuario)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }

            var usuario = _usuarioBusiness.FindById(idUsuario, _idUsuario.Value);
            if (usuario.PerfilUsuario != PerfilUsuario.Administrador)
                return Ok(new List<UsuarioVM>());

            return Ok(_usuarioBusiness.FindAll(_idUsuario.Value));
        }

        [Authorize("Bearer")]
        [HttpPost("GetById")]
        public IActionResult Post([FromBody] int idUsuario)
        {
            bearerToken = HttpContext.Request.Headers["Authorization"].ToString();
            var _idUsuario = ControleAcessoBusinessImpl.getIdUsuarioFromToken(bearerToken);

            if (_idUsuario.Value != idUsuario)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }

            UsuarioVM _usuario = _usuarioBusiness.FindById(idUsuario, _idUsuario.Value);
            if (_usuario == null)
                return BadRequest(new { message ="Usuário não encontrado!" });

            return Ok(_usuario);
        }

        [HttpPost]
        [Authorize("Bearer")]
        public IActionResult Post([FromBody] UsuarioVM usuarioVM)
        {
            bearerToken = HttpContext.Request.Headers["Authorization"].ToString();
            var _idUsuario = ControleAcessoBusinessImpl.getIdUsuarioFromToken(bearerToken);

            if (_idUsuario.Value != usuarioVM.Id)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }

            if (String.IsNullOrEmpty(usuarioVM.Telefone) || String.IsNullOrWhiteSpace(usuarioVM.Telefone))
                return BadRequest("Campo Telefone não pode ser em branco");

            if (String.IsNullOrEmpty(usuarioVM.Email) || String.IsNullOrWhiteSpace(usuarioVM.Email))
                return BadRequest("Campo Login não pode ser em branco");

            if (!IsValidEmail(usuarioVM.Email))
                return BadRequest(new { message = "Email inválido!" });

            return new ObjectResult(_usuarioBusiness.Create(usuarioVM));
        }

        [HttpPut]
        [Authorize("Bearer")]
        public IActionResult Put([FromBody] UsuarioVM usuarioVM)
        {
            bearerToken = HttpContext.Request.Headers["Authorization"].ToString();
            var _idUsuario = ControleAcessoBusinessImpl.getIdUsuarioFromToken(bearerToken);

            if (_idUsuario.Value != usuarioVM.Id)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }

            if (String.IsNullOrEmpty(usuarioVM.Telefone) || String.IsNullOrWhiteSpace(usuarioVM.Telefone))
                return BadRequest("Campo Telefone não pode ser em branco");

            if (String.IsNullOrEmpty(usuarioVM.Email) || String.IsNullOrWhiteSpace(usuarioVM.Email))
                return BadRequest("Campo Login não pode ser em branco");

            if (!IsValidEmail(usuarioVM.Email))
                return BadRequest(new { message = "Email inválido!" });

            UsuarioVM updateUsuario = _usuarioBusiness.Update(usuarioVM);
            if (updateUsuario == null)
                return  BadRequest(new { message = "Usuário não encontrado!" });

            return new ObjectResult(updateUsuario);
        }

        [HttpDelete]
        [Authorize("Bearer")]
        public IActionResult Delete([FromBody] UsuarioVM usuarioVM)
        {
            bearerToken = HttpContext.Request.Headers["Authorization"].ToString();
            var _idUsuario = ControleAcessoBusinessImpl.getIdUsuarioFromToken(bearerToken);

            if (_idUsuario.Value != usuarioVM.Id)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }


            UsuarioVM _usuario = _usuarioBusiness.FindById(usuarioVM.Id, _idUsuario.Value);
            if (_usuario.PerfilUsuario != PerfilUsuario.Administrador)
                return BadRequest(new { message = "Usuário não possui permissão para exectar deleção!" });
                
            _usuarioBusiness.Delete(usuarioVM.Id);
            return NoContent();
        }
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
    }
}

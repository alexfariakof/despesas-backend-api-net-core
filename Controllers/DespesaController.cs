﻿using despesas_backend_api_net_core.Business.Generic;
using despesas_backend_api_net_core.Business.Implementations;
using despesas_backend_api_net_core.Domain.Entities;
using despesas_backend_api_net_core.Domain.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace despesas_backend_api_net_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DespesaController : Controller
    {
        private IBusiness<DespesaVM> _despesaBusiness;
        private string bearerToken;

        public DespesaController(IBusiness<DespesaVM> despesaBusiness)
        {
            _despesaBusiness = despesaBusiness;
        }

        [HttpGet]
        [Authorize("Bearer")]
        public IActionResult Get()
        {
            bearerToken = HttpContext.Request.Headers["Authorization"].ToString();
            var _idUsuario = ControleAcessoBusinessImpl.getIdUsuarioFromToken(bearerToken);

            return Ok(_despesaBusiness.FindAll(_idUsuario.Value));
        }

        [HttpGet("GetById/{id}")]
        [Authorize("Bearer")]
        public IActionResult Get([FromRoute]int id)
        {
            bearerToken = HttpContext.Request.Headers["Authorization"].ToString();
            var _idUsuario = ControleAcessoBusinessImpl.getIdUsuarioFromToken(bearerToken);

            try
            {
                var _despesa = _despesaBusiness.FindById(id, _idUsuario.Value);

                if (_despesa == null)
                    return Ok( new { message = "Nenhuma despesa foi encontrada."});

                return new ObjectResult(new { message = true, despesa = _despesa });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível realizar a consulta da despesa." });
            }
        }

        [HttpGet("GetByIdUsuario/{idUsuario}")]
        [Authorize("Bearer")]
        public IActionResult Post([FromRoute] int idUsuario)
        {
            bearerToken = HttpContext.Request.Headers["Authorization"].ToString();
            var _idUsuario = ControleAcessoBusinessImpl.getIdUsuarioFromToken(bearerToken);

            if (_idUsuario.Value != idUsuario)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }

            if (idUsuario == 0)
                return BadRequest(new { message = "Usuário inexistente!" });
            else
                return Ok(_despesaBusiness.FindByIdUsuario(idUsuario));           

        }

        [HttpPost]
        [Authorize("Bearer")]
        public IActionResult Post([FromBody] DespesaVM despesa)
        {
            bearerToken = HttpContext.Request.Headers["Authorization"].ToString();
            var _idUsuario = ControleAcessoBusinessImpl.getIdUsuarioFromToken(bearerToken);

            if (_idUsuario.Value != despesa.IdUsuario)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }

            if (despesa == null)
                return BadRequest();
            try
            {
                return new ObjectResult(new { message = true, despesa = _despesaBusiness.Create(despesa) });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível realizar o cadastro da despesa."});
            }
        }

        [HttpPut]
        [Authorize("Bearer")]
        public IActionResult Put([FromBody] DespesaVM despesa)
        {
            bearerToken = HttpContext.Request.Headers["Authorization"].ToString();
            var _idUsuario = ControleAcessoBusinessImpl.getIdUsuarioFromToken(bearerToken);

            if (_idUsuario.Value != despesa.IdUsuario)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }


            if (despesa == null)
                return BadRequest();

            var updateDespesa = _despesaBusiness.Update(despesa);
            if (updateDespesa == null)
                return BadRequest(new { message = "Não foi possível atualizar o cadastro da despesa." });

            return new ObjectResult(new { message = true, despesa = updateDespesa });
        }

        [HttpDelete]
        [Authorize("Bearer")]
        public IActionResult Delete([FromBody] DespesaVM despesa)
        {
            bearerToken = HttpContext.Request.Headers["Authorization"].ToString();
            var _idUsuario = ControleAcessoBusinessImpl.getIdUsuarioFromToken(bearerToken);

            if (_idUsuario.Value != despesa.IdUsuario)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }

            if (_despesaBusiness.Delete(despesa.Id))
                return new ObjectResult(new { message = true });
            else
                return BadRequest(new { message = "Erro ao excluir Despesa!" });
        }
    }
}

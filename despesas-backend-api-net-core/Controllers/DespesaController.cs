﻿using despesas_backend_api_net_core.Business.Generic;
using despesas_backend_api_net_core.Domain.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace despesas_backend_api_net_core.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DespesaController : AuthController
    {
        private IBusiness<DespesaVM> _despesaBusiness;
        public DespesaController(IBusiness<DespesaVM> despesaBusiness)
        {
            _despesaBusiness = despesaBusiness;
        }

        [HttpGet]
        [Authorize("Bearer")]
        public IActionResult Get()
        {
            return Ok(_despesaBusiness.FindAll(_idUsuario));
        }

        [HttpGet("GetById/{id}")]
        [Authorize("Bearer")]
        public IActionResult Get([FromRoute]int id)
        {
            try
            {
                var _despesa = _despesaBusiness.FindById(id, _idUsuario);

                if (_despesa == null)
                    return BadRequest( new { message = "Nenhuma despesa foi encontrada."});

                return new OkObjectResult(new { message = true, despesa = _despesa });
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
            if (_idUsuario != idUsuario)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }

            if (idUsuario == 0)
                return BadRequest(new { message = "Usuário inexistente!" });
            else
                return Ok(_despesaBusiness.FindAll(idUsuario));           

        }

        [HttpPost]
        [Authorize("Bearer")]
        public IActionResult Post([FromBody] DespesaVM despesa)
        {
            if (_idUsuario != despesa.IdUsuario)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }

            try
            {
                return new OkObjectResult(new { message = true, despesa = _despesaBusiness.Create(despesa) });
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
            if (_idUsuario != despesa.IdUsuario)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }

            var updateDespesa = _despesaBusiness.Update(despesa);
            if (updateDespesa == null)
                return BadRequest(new { message = "Não foi possível atualizar o cadastro da despesa." });

            return new OkObjectResult(new { message = true, despesa = updateDespesa });
        }

        [HttpDelete("{idDespesa}")]
        [Authorize("Bearer")]
        public IActionResult Delete(int idDespesa)
        {
            DespesaVM despesa = _despesaBusiness.FindById(idDespesa, _idUsuario);
            if (despesa == null || _idUsuario != despesa.IdUsuario)
            {
                return BadRequest(new { message = "Usuário não permitido a realizar operação!" });
            }

            if (_despesaBusiness.Delete(despesa))
                return new OkObjectResult(new { message = true });
            else
                return BadRequest(new { message = "Erro ao excluir Despesa!" });
        }
    }
}
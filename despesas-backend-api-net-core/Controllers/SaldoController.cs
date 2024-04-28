﻿using Business.Abstractions;
using Business.Dtos;
using Business.HyperMedia.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace despesas_backend_api_net_core.Controllers;

[Route("[controller]")]
[ApiController]
public class SaldoController : AuthController
{
    private ISaldoBusiness _saldoBusiness;
    public SaldoController(ISaldoBusiness saldoBusiness)
    {
        _saldoBusiness = saldoBusiness;
    }

    [HttpGet]
    [Authorize("Bearer")]
    [ProducesResponseType((200), Type = typeof(SaldoDto))]
    [ProducesResponseType((400), Type = typeof(string))]
    [ProducesResponseType((401), Type = typeof(UnauthorizedResult))]
    [TypeFilter(typeof(HyperMediaFilter))]
    public IActionResult Get()
    {
        try
        {
            var saldo = _saldoBusiness.GetSaldo(IdUsuario);
            return Ok(saldo);
        }
        catch
        {
            return BadRequest("Erro ao gerar saldo!");
        }
    }

    [HttpGet("ByAno/{ano}")]
    [Authorize("Bearer")]
    [ProducesResponseType((200), Type = typeof(SaldoDto))]
    [ProducesResponseType((400), Type = typeof(string))]
    [ProducesResponseType((401), Type = typeof(UnauthorizedResult))]
    [TypeFilter(typeof(HyperMediaFilter))]
    public IActionResult GetSaldoByAno([FromRoute] DateTime ano)
    {
        try
        {
            var saldo = _saldoBusiness.GetSaldoAnual(ano, IdUsuario);
            return Ok(saldo);
        }
        catch
        {
            return BadRequest("Erro ao gerar saldo!");
        }
    }

    [HttpGet("ByMesAno/{anoMes}")]
    [Authorize("Bearer")]
    [ProducesResponseType((200), Type = typeof(SaldoDto))]
    [ProducesResponseType((400), Type = typeof(string))]
    [ProducesResponseType((401), Type = typeof(UnauthorizedResult))]
    [TypeFilter(typeof(HyperMediaFilter))]
    public IActionResult GetSaldoByMesAno([FromRoute] DateTime anoMes)
    {
        try
        {
            var saldo = _saldoBusiness.GetSaldoByMesAno(anoMes, IdUsuario);
            return Ok(saldo);
        }
        catch
        {
            return BadRequest("Erro ao gerar saldo!");
        }
    }
}

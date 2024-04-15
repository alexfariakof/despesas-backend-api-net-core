﻿using Domain.Entities;
using Business.Dtos.Parser.Interfaces;

namespace Business.Dtos.Parser;
public class LancamentoParser : IParser<LancamentoDto, Lancamento>, IParser<Lancamento, LancamentoDto>
{    
    public Lancamento Parse(Despesa origin)
    {
        if (origin == null) return new Lancamento();
        return new Lancamento
        {
            Id = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0),
            Valor = origin.Valor,
            Data = DateTime.Parse(origin.Data.ToString()),
            Descricao = origin.Descricao,
            UsuarioId = origin.UsuarioId,
            Usuario = origin.Usuario,
            DespesaId = origin.Id,
            Despesa = origin,
            ReceitaId = 0,
            Receita = new Receita (),
            CategoriaId = origin.CategoriaId,
            Categoria = origin.Categoria,
            DataCriacao = DateTime.Now
        };
    }
    public Lancamento Parse(Receita origin)
    {
        if (origin == null) return new Lancamento();
        return new Lancamento
        {
            Id = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0),
            Valor = origin.Valor,
            Data = DateTime.Parse(origin.Data.ToString()),
            Descricao = origin.Descricao,
            UsuarioId = origin.UsuarioId,
            Usuario = origin.Usuario,
            DespesaId = 0,
            Despesa = new Despesa(),
            ReceitaId = origin.Id,
            Receita = origin,
            CategoriaId = origin.CategoriaId,
            Categoria = origin.Categoria,
            DataCriacao = DateTime.Now
        };
    }
    public Lancamento Parse(LancamentoDto origin)
    {
        if (origin == null) return new Lancamento();
        return new Lancamento
        {
            Id = origin.Id,
            DespesaId = origin.IdDespesa,
            ReceitaId = origin.IdReceita,
            UsuarioId = origin.IdUsuario,
            Data = DateTime.Parse(origin.Data),
            DataCriacao = DateTime.Now,
            Valor = origin.Valor,
            Despesa = new Despesa { Id = origin.IdDespesa, Descricao = origin.Descricao },
            Receita = new Receita { Id = origin.IdReceita, Descricao = origin.Descricao }
        };
    }
    public LancamentoDto Parse(Lancamento origin)
    {
        if (origin == null) return new LancamentoDto();
        return new LancamentoDto
        {
            Id = origin.Id,
            IdDespesa = origin.DespesaId.Value,
            IdReceita = origin.ReceitaId.Value,
            IdUsuario = origin.UsuarioId,
            Data = origin.Data.ToShortDateString(),
            Valor = origin.Valor,
            Descricao = origin.Descricao,
            TipoCategoria = origin.DespesaId == 0 ? "Receita" : "Despesa",
            Categoria = origin.Categoria.Descricao
        };
    }
    public List<Lancamento> ParseList(List<LancamentoDto> origin)
    {
        if (origin == null) return new List<Lancamento>();
        return origin.Select(item => Parse(item)).ToList();
    }

    public List<LancamentoDto> ParseList(List<Lancamento> origin)
    {
        if (origin == null) return new List<LancamentoDto>();
        return origin.Select(item => Parse(item)).ToList();
    }
    public List<Lancamento> ParseList(List<Despesa> origin)
    {
        if (origin == null) return new List<Lancamento>();
        return origin.Select(item => Parse(item)).ToList();
    }
    public List<Lancamento> ParseList(List<Receita> origin)
    {
        if (origin == null) return new List<Lancamento>();
        return origin.Select(item => Parse(item)).ToList();
    }
}
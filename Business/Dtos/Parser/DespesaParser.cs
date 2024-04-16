﻿using Business.Dtos.Parser.Interfaces;
using Domain.Entities;

namespace Business.Dtos.Parser;
public class DespesaParser: IParser<DespesaDto, Despesa>, IParser<Despesa, DespesaDto>
{    
    public Despesa Parse(DespesaDto origin)
    {
        if (origin == null) return new Despesa();
        return new Despesa
        {
            Id  = origin.Id,
            Data = origin.Data,
            Descricao = origin.Descricao,                
            Valor = origin.Valor,
            DataVencimento = origin.DataVencimento,
            CategoriaId = origin.Categoria.Id,
            UsuarioId = origin.IdUsuario,
        };
    }

    public DespesaDto Parse(Despesa origin)
    {
        if (origin == null) return new DespesaDto();
        return new DespesaDto
        {
            Id = origin.Id,
            Data = origin.Data,
            Descricao = origin.Descricao,
            Valor = origin.Valor,
            DataVencimento = origin.DataVencimento,
            Categoria = new CategoriaParser().Parse(origin.Categoria),
            IdUsuario = origin.UsuarioId,
            Usuario = new UsuarioParser().Parse(origin.Usuario),                
        };
    }

    public List<Despesa> ParseList(List<DespesaDto> origin)
    {
        if (origin == null) return new List<Despesa>();
        return origin.Select(item => Parse(item)).ToList();
    }

    public List<DespesaDto> ParseList(List<Despesa> origin)
    {
        if (origin == null) return new List<DespesaDto>();
        return origin.Select(item => Parse(item)).ToList();
    }
}
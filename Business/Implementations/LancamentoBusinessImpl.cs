﻿using despesas_backend_api_net_core.Domain.Entities;
using despesas_backend_api_net_core.Domain.VM;
using despesas_backend_api_net_core.Infrastructure.Data.EntityConfig;
using despesas_backend_api_net_core.Infrastructure.Data.Repositories;

namespace despesas_backend_api_net_core.Business.Implementations
{
    public class LancamentoBusinessImpl : ILancamentoBusiness
    {
        private readonly ILancamentoRepositorio _repositorio;

        private readonly LancamentoMap _converter;

        public LancamentoBusinessImpl(ILancamentoRepositorio repositorio)
        {
            _repositorio = repositorio;
            _converter = new LancamentoMap();
        }

        public List<LancamentoVM> FindByMesAno(DateTime data, int idUsuario)
        {
           return  _converter.ParseList(_repositorio.FindByMesAno(data, idUsuario));
        }

        public decimal GetSaldo(int idUsuario)
        {
            return _repositorio.GetSaldo(idUsuario);
        }

        public Grafico GetDadosGraficoByAnoByIdUsuario(int idUsuario, DateTime data)
        {
            return _repositorio.GetDadosGraficoByAno(idUsuario, data);
        }
    }
}

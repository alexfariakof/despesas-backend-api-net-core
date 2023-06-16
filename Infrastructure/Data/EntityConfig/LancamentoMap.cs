using despesas_backend_api_net_core.Domain.VM;
using despesas_backend_api_net_core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using despesas_backend_api_net_core.Infrastructure.ExtensionMethods;
using Google.Protobuf.WellKnownTypes;

namespace despesas_backend_api_net_core.Infrastructure.Data.EntityConfig
{
    public class LancamentoMap : IParser<LancamentoVM, Lancamento>, IParser<Lancamento, LancamentoVM>, IEntityTypeConfiguration<Lancamento>
    {
        public void Configure(EntityTypeBuilder<Lancamento> builder)
        {
            builder.HasKey(m => m.Id);
            
            builder.Property(m => m.UsuarioId)
           .IsRequired();

            builder.Property(m => m.DespesaId)
           .IsRequired(false)
           .HasDefaultValue(null);

            builder.Property(m => m.ReceitaId)
            .IsRequired(false)
            .HasDefaultValue(null);

            builder.Property(m => m.UsuarioId)
           .IsRequired();

            builder.Property(m => m.Data)
            .HasColumnType("timestamp")
            .IsRequired();

            builder.Property(m => m.DataCriacao)
            .HasColumnType("timestamp").HasDefaultValue(null);            

            builder.Property(m => m.Valor)
            .HasColumnType("decimal(10, 2)");

            builder.Property(m => m.Descricao)
            .HasMaxLength(100);            
        }

        public Lancamento Parse(LancamentoVM origin)
        {
            if (origin == null) return new Lancamento();
            return new Lancamento
            {
                Id = origin.Id,
                DespesaId = origin.IdDespesa,
                ReceitaId = origin.IdReceita,
                UsuarioId = origin.IdUsuario,
                Data = origin.Data.ToDateTime(),
                DataCriacao = DateTime.Now,
                Valor = origin.Valor.ToDecimal(),
                Despesa = new Despesa { Id = origin.IdDespesa, Descricao = origin.Descricao },
                Receita = new Receita { Id = origin.IdReceita, Descricao = origin.Descricao },
                Categoria = origin.Categria
            };
        }

        public LancamentoVM Parse(Lancamento origin)
        {
            if (origin == null) return new LancamentoVM();
            return new LancamentoVM
            {
                Id = origin.Id,
                IdDespesa = origin.DespesaId.Value,
                IdReceita = origin.ReceitaId.Value,
                IdUsuario = origin.UsuarioId,
                Data = origin.Data.ToDateBr(),
                Valor = origin.Valor.ToString("N2"),
                Descricao = origin.Descricao,
                TipoCategoria = origin.DespesaId == 0 ? "Receita" : "Despesa"
            };
        }

        public List<Lancamento> ParseList(List<LancamentoVM> origin)
        {
            if (origin == null) return new List<Lancamento>();
            return origin.Select(item => Parse(item)).ToList();
        }

        public List<LancamentoVM> ParseList(List<Lancamento> origin)
        {
            if (origin == null) return new List<LancamentoVM>();
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
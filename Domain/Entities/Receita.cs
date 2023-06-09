﻿namespace despesas_backend_api_net_core.Domain.Entities
{
    public class Receita : BaseModel
    {
        public DateTime Data { get; set; }
        public String Descricao { get; set; }
        public Decimal Valor { get; set; }
        public int UsuarioId { get; set; }
        public virtual Usuario  Usuario { get; set; }
        public int CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }
    }
}
global using Xunit;
global using Moq;
global using despesas_backend_api_net_core.Domain.Entities;
global using despesas_backend_api_net_core.Domain.VM;
global using despesas_backend_api_net_core.Infrastructure.Data.Common;
global using despesas_backend_api_net_core.Infrastructure.Data.Repositories.Generic;
global using despesas_backend_api_net_core.Infrastructure.Data.Repositories.Implementations;
global using Microsoft.EntityFrameworkCore;

public static class Usings
{
    public static List<CategoriaVM> lstCategoriasVM = new List<CategoriaVM>
    {
        new CategoriaVM { Id = 1, IdUsuario = 1, Descricao = "Alimenta��o", IdTipoCategoria = 1 },
        new CategoriaVM { Id = 2,  IdUsuario = 1, Descricao = "Transporte", IdTipoCategoria = 1 },
        new CategoriaVM { Id = 3, IdUsuario = 1, Descricao = "Sal�rio", IdTipoCategoria = 2 },
        new CategoriaVM { Id = 4,  IdUsuario = 1, Descricao = "Lazer", IdTipoCategoria = 1 },
        new CategoriaVM { Id = 5, IdUsuario = 1, Descricao = "Moradia", IdTipoCategoria = 1 },
        new CategoriaVM { Id = 6, IdUsuario = 1, Descricao = "Investimentos", IdTipoCategoria = 2 },
        new CategoriaVM { Id = 7, IdUsuario = 1, Descricao = "Presentes", IdTipoCategoria = 1 },
        new CategoriaVM { Id = 8, IdUsuario = 1, Descricao = "Educa��o", IdTipoCategoria = 1 },
        new CategoriaVM { Id = 9, IdUsuario = 1, Descricao = "Pr�mios", IdTipoCategoria = 2 },
        new CategoriaVM { Id = 10, IdUsuario = 1, Descricao = "Sa�de", IdTipoCategoria = 1 }
    };


    public static List<Categoria> lstCategorias = new List<Categoria>
    {
        new Categoria { Id = 1, Descricao = "Alimenta��o", UsuarioId = 1, TipoCategoria = TipoCategoria.Despesa, Usuario = new Mock<Usuario>().Object  },
        new Categoria { Id = 2, Descricao = "Transporte", UsuarioId= 1, TipoCategoria = TipoCategoria.Despesa, Usuario = new Mock<Usuario>().Object },
        new Categoria { Id = 3, Descricao = "Sal�rio", UsuarioId= 1, TipoCategoria = TipoCategoria.Receita, Usuario = new Mock<Usuario>().Object },
        new Categoria { Id = 4, Descricao = "Lazer", UsuarioId= 1, TipoCategoria = TipoCategoria.Despesa, Usuario = new Mock<Usuario>().Object },
        new Categoria { Id = 5, Descricao = "Moradia", UsuarioId= 1, TipoCategoria = TipoCategoria.Despesa , Usuario = new Mock < Usuario >().Object},
        new Categoria { Id = 6, Descricao = "Investimentos", UsuarioId= 1, TipoCategoria = TipoCategoria.Receita , Usuario = new Mock < Usuario >().Object},
        new Categoria { Id = 7, Descricao = "Presentes", UsuarioId= 1, TipoCategoria = TipoCategoria.Despesa , Usuario = new Mock < Usuario >().Object},
        new Categoria { Id = 8, Descricao = "Educa��o", UsuarioId= 1, TipoCategoria = TipoCategoria.Despesa , Usuario = new Mock < Usuario >().Object},
        new Categoria { Id = 9, Descricao = "Pr�mios", UsuarioId= 1, TipoCategoria = TipoCategoria.Receita , Usuario = new Mock < Usuario >().Object},
        new Categoria { Id = 10, Descricao = "Sa�de", UsuarioId= 1, TipoCategoria = TipoCategoria.Despesa , Usuario = new Mock < Usuario >().Object}
    };

}
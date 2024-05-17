﻿using Business.Dtos.v2;

namespace Fakers.v2;
public class ImagemPerfilUsuarioFaker
{
    static int counter = 1;
    static int counterVM = 1;

    public static ImagemPerfilUsuario GetNewFaker(Usuario usuario)
    {
        var imagemFaker = new Faker<ImagemPerfilUsuario>()
            .RuleFor(i => i.Id, f => counter++)
            .RuleFor(i => i.Url, f => f.Internet.Url())
            .RuleFor(i => i.Name, f => f.System.FileName())
            .RuleFor(i => i.ContentType, f => f.System.CommonFileType())
            .RuleFor(i => i.UsuarioId, usuario.Id)
            .RuleFor(i => i.Usuario, usuario);

        return imagemFaker.Generate();
    }

    public static ImagemPerfilDto GetNewFakerVM(UsuarioDto usuarioDto)
    {
        var imagemFaker = new Faker<ImagemPerfilDto>()
            .RuleFor(i => i.Id, f => counterVM++)
            .RuleFor(i => i.Url, f => f.Internet.Url())
            .RuleFor(i => i.Name, f => f.System.FileName())
            .RuleFor(i => i.UsuarioId, usuarioDto.Id)
            .RuleFor(i => i.ContentType, f => counter % 2 == 0 ? "image/png" : "image/jpg");

        return imagemFaker.Generate();
    }

    public static List<ImagemPerfilUsuario> ImagensPerfilUsuarios(
        Usuario? usuario = null,
        int? idUsuario = null
    )
    {
        var imagens = new List<ImagemPerfilUsuario>();
        for (var i = 0; i < 10; i++)
        {
            if (idUsuario == null)
                usuario = UsuarioFaker.Instance.GetNewFaker();

            var imagem = GetNewFaker(usuario);

            imagens.Add(imagem);
        }
        return imagens;
    }

    public static List<ImagemPerfilDto> ImagensPerfilUsuarioDtos(
        UsuarioDto? usuarioDto = null,
        int? idUsuario = null
    )
    {
        var imagensVM = new List<ImagemPerfilDto>();
        for (var i = 0; i < 10; i++)
        {
            if (idUsuario == null)
                usuarioDto = UsuarioFaker.Instance.GetNewFakerVM();

            var imagemVM = GetNewFakerVM(usuarioDto);

            imagensVM.Add(imagemVM);
        }
        return imagensVM;
    }
}

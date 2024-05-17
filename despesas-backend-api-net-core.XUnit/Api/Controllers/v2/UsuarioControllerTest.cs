﻿using Business.Abstractions;
using Business.Dtos.Parser;
using Business.Dtos.v2;
using despesas_backend_api_net_core.Controllers.v2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Fakers.v2;
using Domain.Entities.ValueObjects;

namespace Api.Controllers.v2;
public class UsuarioControllerTest
{
    protected Mock<IUsuarioBusiness<UsuarioDto>> _mockUsuarioBusiness;
    protected Mock<IImagemPerfilUsuarioBusiness<ImagemPerfilDto, UsuarioDto>> _mockImagemPerfilBusiness;
    protected UsuarioController _usuarioController;
    protected List<UsuarioDto> _usuarioDtos;
    private UsuarioDto administrador;
    private UsuarioDto usuarioNormal;

    private void SetupBearerToken(int idUsuario)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString())
        };
        var identity = new ClaimsIdentity(claims, "IdUsuario");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = claimsPrincipal };
        httpContext.Request.Headers["Authorization"] = "Bearer " + Usings.GenerateJwtToken(idUsuario);
        _usuarioController.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    public UsuarioControllerTest()
    {
        _mockUsuarioBusiness = new Mock<IUsuarioBusiness<UsuarioDto>>();
        _mockImagemPerfilBusiness = new Mock<IImagemPerfilUsuarioBusiness<ImagemPerfilDto, UsuarioDto>>();
        _usuarioController = new UsuarioController(_mockUsuarioBusiness.Object, _mockImagemPerfilBusiness.Object);
        var usuarios = UsuarioFaker.Instance.GetNewFakersUsuarios(20);
        administrador = new UsuarioParser().Parse(usuarios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).First());
        usuarioNormal = new UsuarioParser().Parse(usuarios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Usuario).First());
        _usuarioDtos = new UsuarioParser().ParseList(usuarios);
    }

    [Fact]
    public void Get_With_Usuario_Normal_Returns_BadRequest()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Usuario).Last().Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindAll(idUsuario)).Returns(usauriosVMs.FindAll(u => u.Id == idUsuario));
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usauriosVMs.Find(u => u.Id == idUsuario) ?? new());

        // Act
        var result = _usuarioController.Get() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Usuário não permitido a realizar operação!", message);
        _mockUsuarioBusiness.Verify(b => b.FindAll(idUsuario), Times.Never);
    }

    [Fact]
    public void Get_Should_Returns_OkResult_With_Usuarios()
    {
        // Arrange
        int idUsuario = administrador.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindAll(idUsuario)).Returns(_usuarioDtos.FindAll(u => u.Id == idUsuario));
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(administrador);

        // Act
        var result = _usuarioController.Get() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<List<UsuarioDto>>(result.Value);
        Assert.Equal(_usuarioDtos.FindAll(u => u.Id == idUsuario), result.Value);
        _mockUsuarioBusiness.Verify(b => b.FindAll(idUsuario), Times.Once);
    }

    [Fact]
    public void GetUsuario_Should_Returns_BadRequest()
    {
        // Arrange
        int idUsuario = usuarioNormal.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns((UsuarioDto)null);

        // Act
        var result = _usuarioController.GetUsuario() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Usuário não encontrado!", message);
        _mockUsuarioBusiness.Verify(b => b.FindById(idUsuario), Times.Once);
    }

    [Fact]
    public void GetUsuario_Should_Returns_OkResult_When_Usuario_Normal()
    {
        // Arrange
        int idUsuario = usuarioNormal.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usuarioNormal);

        // Act
        var result = _usuarioController.GetUsuario() as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<UsuarioDto>(result.Value);
        _mockUsuarioBusiness.Verify(b => b.FindById(idUsuario), Times.Once);
    }

    [Fact]
    public void Post_Should_Returns_BadRequest_When_Usuario_Is_Not_Administrador()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Usuario).Last());
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(It.IsAny<int>())).Throws(new ArgumentException("Usuário não permitido a realizar operação!"));
        _mockUsuarioBusiness.Setup(business => business.Create(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Usuário não permitido a realizar operação!"));


        // Act
        var result = _usuarioController.Post(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Usuário não permitido a realizar operação!", message);
        _mockUsuarioBusiness.Verify(b => b.FindById(idUsuario), Times.Never);
        _mockUsuarioBusiness.Verify(b => b.Create(usuarioDto), Times.Once);
    }

    [Fact]
    public void Post_Should_Returns_OkResult_When_Usuario_Is_Administrador()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usauriosVMs.Find(u => u.Id == idUsuario) ?? new());
        _mockUsuarioBusiness.Setup(business => business.Create(usuarioDto)).Returns(usuarioDto);

        // Act
        var result = _usuarioController.Post(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<UsuarioDto>(result.Value);
        _mockUsuarioBusiness.Verify(b => b.Create(usuarioDto), Times.Once);
    }

    [Fact]
    public void Put_Should_Update_UsuarioDto()
    {
        // Arrange
        var usuarioDto = _usuarioDtos[4];
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.Update(usuarioDto)).Returns(usuarioDto);

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<UsuarioDto>(result.Value);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Once);
    }

    [Fact]
    public void Delete_Should_Return_True()
    {
        // Arrange
        var usuarioDto = usuarioNormal;
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(administrador.Id);
        _mockUsuarioBusiness.Setup(business => business.Delete(usuarioDto)).Returns(true);
        _mockUsuarioBusiness.Setup(business => business.FindById(administrador.Id)).Returns(administrador);

        // Act
        var result = _usuarioController.Delete(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)result.Value);
        _mockUsuarioBusiness.Verify(b => b.Delete(usuarioDto), Times.Once);
    }

    [Fact]
    public void Post_Should_Returns_BadRequest_When_Telefone_IsNull()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        usuarioDto.Telefone = null;
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(It.IsAny<int>())).Throws(new ArgumentException("Campo Telefone não pode ser em branco"));
        _mockUsuarioBusiness.Setup(business => business.Create(usuarioDto)).Throws(new ArgumentException("Campo Telefone não pode ser em branco"));

        // Act
        var result = _usuarioController.Post(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Telefone não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Create(usuarioDto), Times.Once);
    }

    [Fact]
    public void Post_Should_Returns_BadRequest_When_Email_IsNull()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        usuarioDto.Email = null;
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Throws(new ArgumentException("Campo Login não pode ser em branco"));
        _mockUsuarioBusiness.Setup(business => business.Create(usuarioDto)).Throws(new ArgumentException("Campo Login não pode ser em branco"));

        // Act
        var result = _usuarioController.Post(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Login não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Create(usuarioDto), Times.Once);
    }

    [Fact]
    public void Post_Should_Returns_BadRequest_When_Email_IsNullOrWhiteSpace()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        usuarioDto.Email = "  ";
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Throws(new ArgumentException("Campo Login não pode ser em branco"));
        _mockUsuarioBusiness.Setup(business => business.Create(usuarioDto)).Throws(new ArgumentException("Campo Login não pode ser em branco"));

        // Act
        var result = _usuarioController.Post(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Login não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Create(usuarioDto), Times.Once);
    }

    [Fact]
    public void Post_Should_Returns_BadRequest_When_Email_IsInvalid()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        usuarioDto.Email = "TestINvalidemail";
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Throws(new ArgumentException("Email inválido!"));
        _mockUsuarioBusiness.Setup(business => business.Create(usuarioDto)).Throws(new ArgumentException("Email inválido!"));

        // Act
        var result = _usuarioController.Post(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Email inválido!", message);
        _mockUsuarioBusiness.Verify(b => b.Create(usuarioDto), Times.Once);
    }

    [Fact]
    public void Put_Should_Returns_BadRequest_When_Telefone_IsNull()
    {
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).First());
        usuarioDto.Telefone = null;
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Campo Telefone não pode ser em branco"));

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Telefone não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Once);
    }

    [Fact]
    public void Put_Should_Returns_BadRequest_When_Email_IsNull()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        usuarioDto.Email = string.Empty;
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Campo Login não pode ser em branco"));

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Login não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Once);
    }

    [Fact]
    public void Put_Should_Returns_BadRequest_When_Email_IsNullOrWhiteSpace()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        usuarioDto.Email = " ";
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Campo Login não pode ser em branco"));

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Login não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Once);
    }

    [Fact]
    public void Put_Should_Returns_BadRequest_When_Email_IsInvalid()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        usuarioDto.Email = "invalidEmail.com";
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Email inválido!"));

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Email inválido!", message);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Once);
    }

    [Fact]
    public void Put_Should_Returns_BadRequest_When_Usuario_IsNull()
    {
        // Arrange
        var usuarioDto = _usuarioDtos.First();
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.Update(usuarioDto)).Returns((UsuarioDto)null);

        // Act
        var result = _usuarioController.Put(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Usuário não encontrado!", message);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Once);
    }

    [Fact]
    public void Delete_Should_Returns_BadRequest_When_Usuario_IsNotAdministrador()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Usuario).Last());
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usauriosVMs.Find(u => u.Id == idUsuario) ?? new());
        _mockUsuarioBusiness.Setup(business => business.Delete(usuarioNormal)).Returns(false);

        // Act
        var result = _usuarioController.Delete(usuarioNormal) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Usuário não permitido a realizar operação!", message);
        _mockUsuarioBusiness.Verify(b => b.FindById(idUsuario), Times.Once);
        _mockUsuarioBusiness.Verify(b => b.Delete(usuarioNormal), Times.Never);
    }

    [Fact]
    public void Delete_Should_Returns_BadRequest_When_Try_To_Delete_Usuario()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usauriosVMs.Find(u => u.Id == idUsuario) ?? new());
        _mockUsuarioBusiness.Setup(business => business.Delete(usuarioNormal)).Returns(false);

        // Act
        var result = _usuarioController.Delete(usuarioNormal) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Erro ao excluir Usuário!", message);
        _mockUsuarioBusiness.Verify(b => b.FindById(idUsuario), Times.Once);
        _mockUsuarioBusiness.Verify(b => b.Delete(usuarioNormal), Times.Once);
    }

    [Fact]
    public void PutAdministrador_Should_Update_UsuarioDto()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usauriosVMs.Find(u => u.Id == idUsuario) ?? new());
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.Update(usuarioDto)).Returns(usuarioDto);

        // Act
        var result = _usuarioController.PutAdministrador(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        Assert.IsType<UsuarioDto>(result.Value);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Once);
    }

    [Fact]
    public void PutAdministrador_Should_Returns_BadRequest_When_Email_IsInvalid()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        usuarioDto.Email = "TestINvalidemail";
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usauriosVMs.Find(u => u.Id == idUsuario) ?? new());
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Email inválido!"));

        // Act
        var result = _usuarioController.PutAdministrador(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Email inválido!", message);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Once );
    }

    [Fact]
    public void PutAdministrador_Should_Returns_BadRequest_When_Telefone_IsNull()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        usuarioDto.Telefone = null;
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usauriosVMs.Find(u => u.Id == idUsuario) ?? new());
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Campo Telefone não pode ser em branco"));

        // Act
        var result = _usuarioController.PutAdministrador(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Telefone não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Once);
    }

    [Fact]
    public void PutAdministrador_Should_Returns_BadRequest_When_Email_IsNull()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        usuarioDto.Email = null;
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usauriosVMs.Find(u => u.Id == idUsuario) ?? new());
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Campo Login não pode ser em branco"));

        // Act
        var result = _usuarioController.PutAdministrador(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Login não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Once);
    }

    [Fact]
    public void PutAdministrador_Should_Returns_BadRequest_When_Email_IsNullOrWhiteSpace()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(20);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        usuarioDto.Email = " ";
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usauriosVMs.Find(u => u.Id == idUsuario) ?? new());
        _mockUsuarioBusiness.Setup(business => business.Update(It.IsAny<UsuarioDto>())).Throws(new ArgumentException("Campo Login não pode ser em branco"));

        // Act
        var result = _usuarioController.PutAdministrador(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Campo Login não pode ser em branco", message);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Once);
    }

    [Fact]
    public void PutAdministrador_Should_Returns_BadRequest_When_Usuario_IsNull()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(10);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Administrador).Last());
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usauriosVMs.Find(u => u.Id == idUsuario) ?? new());
        _mockUsuarioBusiness.Setup(business => business.Update(usuarioDto)).Returns((UsuarioDto)null);

        // Act
        var result = _usuarioController.PutAdministrador(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Usuário não encontrado!", message);
        _mockUsuarioBusiness.Verify(b => b.Update(It.IsAny<UsuarioDto>()), Times.Once);
    }

    [Fact]
    public void PutAdministrador_Should_Returns_BadRequest_When_Usuario_Is_Not_Administrador()
    {
        // Arrange
        var usaurios = UsuarioFaker.Instance.GetNewFakersUsuarios(15);
        var usuarioDto = new UsuarioParser().Parse(usaurios.FindAll(u => u.PerfilUsuario == PerfilUsuario.PerfilType.Usuario).First());
        var usauriosVMs = new UsuarioParser().ParseList(usaurios);
        int idUsuario = usuarioDto.Id;
        SetupBearerToken(idUsuario);
        _mockUsuarioBusiness.Setup(business => business.FindById(idUsuario)).Returns(usauriosVMs.Find(u => u.Id == idUsuario) ?? new());
        _mockUsuarioBusiness.Setup(business => business.Update(usuarioDto)).Returns((UsuarioDto)null);

        // Act
        var result = _usuarioController.PutAdministrador(usuarioDto) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result);
        var message = result.Value;
        Assert.Equal("Usuário não permitido a realizar operação!", message);
        _mockUsuarioBusiness.Verify(b => b.Update(usuarioDto), Times.Never);
    }
}

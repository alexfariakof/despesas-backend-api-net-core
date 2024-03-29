﻿using despesas_backend_api_net_core.Business;
using despesas_backend_api_net_core.Controllers;
using despesas_backend_api_net_core.Infrastructure.Data.EntityConfig;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Xunit.Extensions.Ordering;

namespace Controllers.Usuario
{
    [Order(6)]
    public class ImagemPerfilUsuarioControllerTest
    {
        protected Mock<IUsuarioBusiness> _mockUsuarioBusiness;
        protected Mock<IImagemPerfilUsuarioBusiness> _mockImagemPerfilBusiness;
        protected UsuarioController _usuarioController;
        protected List<UsuarioVM> _usuarioVMs;

        private void SetupBearerToken(int idUsuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, idUsuario.ToString())
            };
            var identity = new ClaimsIdentity(claims, "IdUsuario");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            httpContext.Request.Headers["Authorization"] =
                "Bearer " + Usings.GenerateJwtToken(idUsuario);

            _usuarioController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        public ImagemPerfilUsuarioControllerTest()
        {
            _mockUsuarioBusiness = new Mock<IUsuarioBusiness>();
            _mockImagemPerfilBusiness = new Mock<IImagemPerfilUsuarioBusiness>();
            _usuarioController = new UsuarioController(
                _mockUsuarioBusiness.Object,
                _mockImagemPerfilBusiness.Object
            );
        }

        [Fact, Order(1)]
        public void GetImage_Should_Returns_OkResults_With_ImagemPerfilUsuario()
        {
            // Arrange
            var _imagemPerfilUsuarios = ImagemPerfilUsuarioFaker.ImagensPerfilUsuarios();
            var _imagemPerfilUsuarioVMs = new ImagemPerfilUsuarioMap().ParseList(
                _imagemPerfilUsuarios
            );
            var usuarioVM = new UsuarioMap().Parse(_imagemPerfilUsuarios.First().Usuario);
            int idUsuario = usuarioVM.Id;
            SetupBearerToken(idUsuario);
            _mockImagemPerfilBusiness
                .Setup(business => business.FindAll(idUsuario))
                .Returns(_imagemPerfilUsuarioVMs);

            // Act
            var result = _usuarioController.GetImage() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var value = result.Value;

            var message = (bool)value?.GetType()?.GetProperty("message")?.GetValue(value, null);

            Assert.True(message);
            var _imagemPerfilUsuario =
                value?.GetType()?.GetProperty("imagemPerfilUsuario")?.GetValue(value, null)
                as ImagemPerfilVM;
            Assert.NotNull(_imagemPerfilUsuario);
            Assert.IsType<ImagemPerfilVM>(_imagemPerfilUsuario);
            _mockImagemPerfilBusiness.Verify(b => b.FindAll(idUsuario), Times.Once);
        }

        [Fact, Order(2)]
        public void Get_Should_Returns_BadRequest_When_ImagemPerfilUsuario_NotFound()
        {
            // Arrange
            var _imagemPerfilUsuarios = ImagemPerfilUsuarioFaker.ImagensPerfilUsuarios();
            var _imagemPerfilUsuarioVMs = new ImagemPerfilUsuarioMap().ParseList(
                _imagemPerfilUsuarios
            );
            var usuarioVM = new UsuarioMap().Parse(_imagemPerfilUsuarios.First().Usuario);
            int idUsuario = 987654;
            SetupBearerToken(idUsuario);
            _mockImagemPerfilBusiness
                .Setup(business => business.FindAll(idUsuario))
                .Returns(_imagemPerfilUsuarioVMs);

            // Act
            var result = _usuarioController.GetImage() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;
            var message = value?.GetType()?.GetProperty("message")?.GetValue(value, null) as string;
            Assert.Equal("Usuário não possui nenhuma imagem de perfil cadastrada!", message);
            _mockImagemPerfilBusiness.Verify(b => b.FindAll(idUsuario), Times.Once);
        }

        [Fact, Order(3)]
        public async void Post_Should_Create_And_Returns_OkResult_For_ImagesTypes_JPG_PNG_JPEG()
        {
            // Arrange
            // Arrange
            var _imagemPerfilUsuarios = ImagemPerfilUsuarioFaker.ImagensPerfilUsuarios();
            var _imagemPerfilUsuarioVMs = new ImagemPerfilUsuarioMap().ParseList(
                _imagemPerfilUsuarios
            );
            var imagemPerfilUsuarioVM = _imagemPerfilUsuarioVMs.First();

            int idUsuario = imagemPerfilUsuarioVM.IdUsuario;

            SetupBearerToken(idUsuario);
            _mockImagemPerfilBusiness
                .Setup(business => business.Create(It.IsAny<ImagemPerfilVM>()))
                .Returns(imagemPerfilUsuarioVM);

            var formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file content")),
                0,
                Encoding.UTF8.GetBytes("Test file content").Length,
                "test",
                "test.jpg"
            );
            formFile.Headers = new HeaderDictionary { { "Content-Type", "image/jpg" } };

            // Act
            var result = await _usuarioController.PostImagemPerfil(formFile) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var value = result.Value;

            var message = (bool)value?.GetType()?.GetProperty("message")?.GetValue(value, null);

            Assert.True(message);
            var imagemPerfilUsuario =
                value?.GetType()?.GetProperty("imagemPerfilUsuario")?.GetValue(value, null)
                as ImagemPerfilVM;
            Assert.NotNull(imagemPerfilUsuario);
            Assert.IsType<ImagemPerfilVM>(imagemPerfilUsuario);
            _mockImagemPerfilBusiness.Verify(b => b.Create(It.IsAny<ImagemPerfilVM>()), Times.Once);

            // Arrage file type PNG
            formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file content png")),
                0,
                Encoding.UTF8.GetBytes("Test file content png").Length,
                "Test File PNG",
                "test.png"
            );
            formFile.Headers = new HeaderDictionary { { "Content-Type", "image/png" } };

            // Act file type PNG
            result = await _usuarioController.PostImagemPerfil(formFile) as ObjectResult;

            // Assert file Type PNG
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            value = result.Value;

            message = (bool)value?.GetType()?.GetProperty("message")?.GetValue(value, null);

            Assert.True(message);
            imagemPerfilUsuario =
                value?.GetType()?.GetProperty("imagemPerfilUsuario")?.GetValue(value, null)
                as ImagemPerfilVM;
            Assert.NotNull(imagemPerfilUsuario);
            Assert.IsType<ImagemPerfilVM>(imagemPerfilUsuario);
            _mockImagemPerfilBusiness.Verify(
                b => b.Create(It.IsAny<ImagemPerfilVM>()),
                Times.Exactly(2)
            );

            // Arrage file type JPEG
            formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file contentjpeg")),
                0,
                Encoding.UTF8.GetBytes("Test file content jpeg").Length,
                "Test File JPEG",
                "test.jpeg"
            );
            formFile.Headers = new HeaderDictionary { { "Content-Type", "image/jpeg" } };

            // Act file type JPEG
            result = await _usuarioController.PostImagemPerfil(formFile) as ObjectResult;

            // Assert file Type JPEG
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            value = result.Value;

            message = (bool)value?.GetType()?.GetProperty("message")?.GetValue(value, null);

            Assert.True(message);
            imagemPerfilUsuario =
                value?.GetType()?.GetProperty("imagemPerfilUsuario")?.GetValue(value, null)
                as ImagemPerfilVM;
            Assert.NotNull(imagemPerfilUsuario);
            Assert.IsType<ImagemPerfilVM>(imagemPerfilUsuario);
            _mockImagemPerfilBusiness.Verify(
                b => b.Create(It.IsAny<ImagemPerfilVM>()),
                Times.Exactly(3)
            );
        }

        [Fact, Order(4)]
        public async void Post_Should_Returns_BadRequest_For_Invalid_Images_Type()
        {
            // Arrange
            var imagemPerfilUsuarioVM = ImagemPerfilUsuarioFaker.ImagensPerfilUsuarioVMs().First();

            int idUsuario = imagemPerfilUsuarioVM.IdUsuario;

            SetupBearerToken(idUsuario);
            _mockImagemPerfilBusiness
                .Setup(business => business.Create(It.IsAny<ImagemPerfilVM>()))
                .Returns(imagemPerfilUsuarioVM);

            var formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file not Image type content")),
                0,
                Encoding.UTF8.GetBytes("Test file not Image type content").Length,
                "DATA File Erro",
                "test.txt"
            );
            formFile.Headers = new HeaderDictionary { { "Content-Type", "image/txt" } };

            // Act
            var result = await _usuarioController.PostImagemPerfil(formFile) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;
            var message = value?.GetType()?.GetProperty("message")?.GetValue(value, null) as String;
            Assert.Equal("Apenas arquivos do tipo jpg, jpeg ou png são aceitos.", message);
            _mockImagemPerfilBusiness.Verify(
                b => b.Create(It.IsAny<ImagemPerfilVM>()),
                Times.Never
            );
        }

        [Fact, Order(5)]
        public async void Post_Should_Try_Create_And_Returns_BadRequest()
        {
            // Arrange
            var imagemPerfilUsuarioVM = ImagemPerfilUsuarioFaker.ImagensPerfilUsuarioVMs().First();

            int idUsuario = imagemPerfilUsuarioVM.IdUsuario;

            SetupBearerToken(idUsuario);

            _mockImagemPerfilBusiness
                .Setup(business => business.Create(It.IsAny<ImagemPerfilVM>()))
                .Returns((ImagemPerfilVM)null);

            var formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file content")),
                0,
                Encoding.UTF8.GetBytes("Test file content").Length,
                "test",
                "test.jpg"
            );
            formFile.Headers = new HeaderDictionary { { "Content-Type", "image/jpg" } };

            // Act
            var result = await _usuarioController.PostImagemPerfil(formFile) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;
            value = result.Value;

            var message = (bool)value?.GetType()?.GetProperty("message")?.GetValue(value, null);

            Assert.False(message);
            var imagemPerfilUsuario =
                value?.GetType()?.GetProperty("imagemPerfilUsuario")?.GetValue(value, null)
                as ImagemPerfilVM;
            Assert.Null(imagemPerfilUsuario);
            _mockImagemPerfilBusiness.Verify(b => b.Create(It.IsAny<ImagemPerfilVM>()), Times.Once);
        }

        [Fact, Order(6)]
        public async void Post_Throws_Erro_And_Returns_BadRequest()
        {
            // Arrange

            int idUsuario = 1;

            SetupBearerToken(1);

            _mockImagemPerfilBusiness
                .Setup(business => business.Create(It.IsAny<ImagemPerfilVM>()))
                .Returns((ImagemPerfilVM)null);

            var formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file content")),
                0,
                Encoding.UTF8.GetBytes("Test file content").Length,
                "test",
                "test.jpg"
            );

            // Act
            var result = await _usuarioController.PostImagemPerfil(formFile) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;
            value = result.Value;
            //var message = value?.GetType()?.GetProperty("message")?.GetValue(value, null) as String;
            //Assert.Equal("Erro ao incluir nova imagem de peefil!", message);
            _mockImagemPerfilBusiness.Verify(
                b => b.Create(It.IsAny<ImagemPerfilVM>()),
                Times.Never
            );
        }

        [Fact, Order(7)]
        public async void Put_Should_Returns_OkResult_For_ImagesTypes_JPG_PNG_JPEG()
        {
            // Arrange
            var imagemPerfilUsuarioVM = ImagemPerfilUsuarioFaker.ImagensPerfilUsuarioVMs().First();

            int idUsuario = imagemPerfilUsuarioVM.IdUsuario;

            SetupBearerToken(idUsuario);
            _mockImagemPerfilBusiness
                .Setup(business => business.Update(It.IsAny<ImagemPerfilVM>()))
                .Returns(imagemPerfilUsuarioVM);

            var formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file content")),
                0,
                Encoding.UTF8.GetBytes("Test file content").Length,
                "test",
                "test.jpg"
            );
            formFile.Headers = new HeaderDictionary { { "Content-Type", "image/jpg" } };

            // Act
            var result = await _usuarioController.PutImagemPerfil(formFile) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var value = result.Value;

            var message = (bool)value?.GetType()?.GetProperty("message")?.GetValue(value, null);

            Assert.True(message);
            var imagemPerfilUsuario =
                value?.GetType()?.GetProperty("imagemPerfilUsuario")?.GetValue(value, null)
                as ImagemPerfilVM;
            Assert.NotNull(imagemPerfilUsuario);
            Assert.IsType<ImagemPerfilVM>(imagemPerfilUsuario);
            _mockImagemPerfilBusiness.Verify(b => b.Update(It.IsAny<ImagemPerfilVM>()), Times.Once);

            // Arrage file type PNG
            formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file content png")),
                0,
                Encoding.UTF8.GetBytes("Test file content png").Length,
                "Test File PNG",
                "test.png"
            );
            formFile.Headers = new HeaderDictionary { { "Content-Type", "image/png" } };

            // Act file type PNG
            result = await _usuarioController.PutImagemPerfil(formFile) as ObjectResult;

            // Assert file Type PNG
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            value = result.Value;

            message = (bool)value?.GetType()?.GetProperty("message")?.GetValue(value, null);

            Assert.True(message);
            imagemPerfilUsuario =
                value?.GetType()?.GetProperty("imagemPerfilUsuario")?.GetValue(value, null)
                as ImagemPerfilVM;
            Assert.NotNull(imagemPerfilUsuario);
            Assert.IsType<ImagemPerfilVM>(imagemPerfilUsuario);
            _mockImagemPerfilBusiness.Verify(
                b => b.Update(It.IsAny<ImagemPerfilVM>()),
                Times.Exactly(2)
            );

            // Arrage file type JPEG
            formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file contentjpeg")),
                0,
                Encoding.UTF8.GetBytes("Test file content jpeg").Length,
                "Test File JPEG",
                "test.jpeg"
            );
            formFile.Headers = new HeaderDictionary { { "Content-Type", "image/jpeg" } };

            // Act file type JPEG
            result = await _usuarioController.PutImagemPerfil(formFile) as ObjectResult;

            // Assert file Type JPEG
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            value = result.Value;

            message = (bool)value?.GetType()?.GetProperty("message")?.GetValue(value, null);

            Assert.True(message);
            imagemPerfilUsuario =
                value?.GetType()?.GetProperty("imagemPerfilUsuario")?.GetValue(value, null)
                as ImagemPerfilVM;
            Assert.NotNull(imagemPerfilUsuario);
            Assert.IsType<ImagemPerfilVM>(imagemPerfilUsuario);
            _mockImagemPerfilBusiness.Verify(
                b => b.Update(It.IsAny<ImagemPerfilVM>()),
                Times.Exactly(3)
            );
        }

        [Fact, Order(8)]
        public async void Put_Throws_Erro_And_Returns_BadRequest()
        {
            // Arrange

            int idUsuario = 1;

            SetupBearerToken(1);

            _mockImagemPerfilBusiness
                .Setup(business => business.Update(It.IsAny<ImagemPerfilVM>()))
                .Returns((ImagemPerfilVM)null);

            var formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file content")),
                0,
                Encoding.UTF8.GetBytes("Test file content").Length,
                "test",
                "test.jpg"
            );

            // Act
            var result = await _usuarioController.PutImagemPerfil(formFile) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;
            value = result.Value;
            //var message = value?.GetType()?.GetProperty("message")?.GetValue(value, null) as String;
            //Assert.Equal("Erro ao Atualizar imagem do perfil!", message);
            _mockImagemPerfilBusiness.Verify(
                b => b.Update(It.IsAny<ImagemPerfilVM>()),
                Times.Never
            );
        }

        [Fact, Order(9)]
        public async void Put_Should_Returns_BadRequest_For_Invalid_Images_Type()
        {
            // Arrange
            var imagemPerfilUsuarioVM = ImagemPerfilUsuarioFaker.ImagensPerfilUsuarioVMs().First();

            int idUsuario = imagemPerfilUsuarioVM.IdUsuario;

            SetupBearerToken(idUsuario);
            _mockImagemPerfilBusiness
                .Setup(business => business.Update(It.IsAny<ImagemPerfilVM>()))
                .Returns(imagemPerfilUsuarioVM);

            var formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file not Image type content")),
                0,
                Encoding.UTF8.GetBytes("Test file not Image type content").Length,
                "DATA File Erro",
                "test.txt"
            );
            formFile.Headers = new HeaderDictionary { { "Content-Type", "image/txt" } };

            // Act
            var result = await _usuarioController.PutImagemPerfil(formFile) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;
            var message = value?.GetType()?.GetProperty("message")?.GetValue(value, null) as String;
            Assert.Equal("Apenas arquivos do tipo jpg, jpeg ou png são aceitos.", message);
            _mockImagemPerfilBusiness.Verify(
                b => b.Update(It.IsAny<ImagemPerfilVM>()),
                Times.Never
            );
        }

        [Fact, Order(10)]
        public async void Put_Should_Returns_BadRequest_When_ImagemPerfil_IsNull()
        {
            // Arrange
            var imagemPerfilUsuarioVM = ImagemPerfilUsuarioFaker.ImagensPerfilUsuarioVMs().First();

            int idUsuario = imagemPerfilUsuarioVM.IdUsuario;

            SetupBearerToken(idUsuario);

            _mockImagemPerfilBusiness
                .Setup(business => business.Update(It.IsAny<ImagemPerfilVM>()))
                .Returns((ImagemPerfilVM)null);

            var formFile = new FormFile(
                new MemoryStream(Encoding.UTF8.GetBytes("Test file content")),
                0,
                Encoding.UTF8.GetBytes("Test file content").Length,
                "test",
                "test.jpg"
            );
            formFile.Headers = new HeaderDictionary { { "Content-Type", "image/jpg" } };

            // Act
            var result = await _usuarioController.PutImagemPerfil(formFile) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;
            value = result.Value;

            var message = (bool)value?.GetType()?.GetProperty("message")?.GetValue(value, null);

            Assert.False(message);
            var imagemPerfilUsuario =
                value?.GetType()?.GetProperty("imagemPerfilUsuario")?.GetValue(value, null)
                as ImagemPerfilVM;
            Assert.Null(imagemPerfilUsuario);
            _mockImagemPerfilBusiness.Verify(b => b.Update(It.IsAny<ImagemPerfilVM>()), Times.Once);
        }

        [Fact, Order(11)]
        public void Delete_Should_Returns_OkResults()
        {
            // Arrange
            int idUsuario = 1;
            SetupBearerToken(idUsuario);
            _mockImagemPerfilBusiness
                .Setup(business => business.Delete(It.IsAny<int>()))
                .Returns(true);

            // Act
            var result = _usuarioController.DeleteImagemPerfil() as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var value = result.Value;
            var message = value?.GetType()?.GetProperty("message")?.GetValue(value, null);
            Assert.IsType<bool>(message);
            Assert.True((bool)message);
            _mockImagemPerfilBusiness.Verify(b => b.Delete(It.IsAny<int>()), Times.Once);
        }

        [Fact, Order(13)]
        public void Delete_Should_Returns_BadRequest_When_Try_To_Delete()
        {
            // Arrange
            int idUsuario = 1;
            SetupBearerToken(idUsuario);
            _mockImagemPerfilBusiness
                .Setup(business => business.Delete(It.IsAny<int>()))
                .Returns(false);

            // Act
            var result = _usuarioController.DeleteImagemPerfil() as ObjectResult;

            // Assert
            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;
            value = result.Value;
            var message = value?.GetType()?.GetProperty("message")?.GetValue(value, null);
            Assert.IsType<bool>(message);
            Assert.False((bool)message);
            _mockImagemPerfilBusiness.Verify(b => b.Delete(It.IsAny<int>()), Times.Once);
        }

        [Fact, Order(14)]
        public void Delete_Throws_Erro_And_Retuns_BadRequestResult()
        {
            // Arrange
            int idUsuario = 1;
            SetupBearerToken(idUsuario);
            _mockImagemPerfilBusiness
                .Setup(business => business.Delete(It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            var result = _usuarioController.DeleteImagemPerfil() as ObjectResult;

            // Assert
            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;
            value = result.Value;
            var message = value?.GetType()?.GetProperty("message")?.GetValue(value, null) as String;
            Assert.Equal("Erro ao excluir imagem do perfil!", message);
            _mockImagemPerfilBusiness.Verify(b => b.Delete(It.IsAny<int>()), Times.Once);
        }
    }
}

﻿using despesas_backend_api_net_core.Business.Generic;
using despesas_backend_api_net_core.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Xunit.Extensions.Ordering;

namespace Controllers
{
    [Order(2)]
    public class CategoriaControllerTest
    {
        protected Mock<IBusiness<CategoriaVM>> _mockCategoriaBusiness;
        protected CategoriaController _categoriaController;

        private void SetupBearerToken(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "IdUsuario");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            httpContext.Request.Headers["Authorization"] =
                "Bearer " + Usings.GenerateJwtToken(userId);

            _categoriaController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        public CategoriaControllerTest()
        {
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
        }

        [Fact, Order(1)]
        public void Get_Returns_Ok_Result()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var categoriaVMs = CategoriaFaker.CategoriasVMs();

            var idUsuario = categoriaVMs.First().IdUsuario;

            SetupBearerToken(idUsuario);
            _mockCategoriaBusiness
                .Setup(b => b.FindAll(idUsuario))
                .Returns(categoriaVMs.FindAll(c => c.IdUsuario == idUsuario));

            // Act
            var result = _categoriaController.Get() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<CategoriaVM>>(result.Value);
            Assert.Equal(categoriaVMs.FindAll(c => c.IdUsuario == idUsuario), result.Value);
        }

        [Fact, Order(2)]
        public void GetById_Returns_Ok_Result()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var categoriaVM = CategoriaFaker.CategoriasVMs().First();

            var idCategoria = categoriaVM.IdUsuario;

            SetupBearerToken(idCategoria);

            _mockCategoriaBusiness
                .Setup(b => b.FindById(idCategoria, categoriaVM.IdUsuario))
                .Returns(categoriaVM);

            // Act
            var result = _categoriaController.GetById(idCategoria) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CategoriaVM>(result.Value);
            Assert.Equal(categoriaVM, result.Value);
        }

        [Fact, Order(4)]
        public void GetByTipoCategoria_Returns_Ok_Result_TipoCategoria_Todas()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var listCategoriaVM = CategoriaFaker.CategoriasVMs();

            var idUsuario = listCategoriaVM.FirstOrDefault().Id;

            SetupBearerToken(idUsuario);
            var tipoCategoria = TipoCategoria.Todas;
            _mockCategoriaBusiness.Setup(b => b.FindAll(idUsuario)).Returns(listCategoriaVM);

            // Act
            var result = _categoriaController.GetByTipoCategoria(tipoCategoria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<CategoriaVM>>(result.Value);
        }

        [Fact, Order(5)]
        public void GetByTipoCategoria_Returns_Ok_Result()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var listCategoriaVM = CategoriaFaker.CategoriasVMs();

            var idUsuario = listCategoriaVM.FirstOrDefault().Id;

            SetupBearerToken(idUsuario);
            var tipoCategoria = TipoCategoria.Despesa;
            _mockCategoriaBusiness.Setup(b => b.FindAll(idUsuario)).Returns(listCategoriaVM);

            // Act
            var result = _categoriaController.GetByTipoCategoria(tipoCategoria) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<List<CategoriaVM>>(result.Value);
        }

        [Fact, Order(6)]
        public void Post_Returns_Ok_Result()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var obj = CategoriaFaker.CategoriasVMs().First();
            var categoriaVM = new CategoriaVM
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                IdUsuario = 1,
                IdTipoCategoria = (int)TipoCategoria.Despesa
            };
            SetupBearerToken(categoriaVM.IdUsuario);
            _mockCategoriaBusiness.Setup(b => b.Create(categoriaVM)).Returns(categoriaVM);

            // Act
            var result = _categoriaController.Post(categoriaVM) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var value = result.Value;

            var categoria = value.GetType().GetProperty("categoria").PropertyType;

            Assert.NotNull(categoria);
            value = result.Value;

            var message = (bool)value.GetType().GetProperty("message").GetValue(value, null);

            Assert.True(message);
        }

        [Fact, Order(8)]
        public void Post_Returns_Bad_Request_When_TipoCategoria_Todas()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var obj = CategoriaFaker.CategoriasVMs().First();
            var categoriaVM = new CategoriaVM
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                IdUsuario = obj.IdUsuario,
                IdTipoCategoria = (int)TipoCategoria.Todas
            };

            SetupBearerToken(categoriaVM.IdUsuario);

            _mockCategoriaBusiness.Setup(b => b.Create(categoriaVM)).Returns(categoriaVM);

            // Act
            var result = _categoriaController.Post(categoriaVM) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;

            var message = value.GetType().GetProperty("message").GetValue(value, null) as string;

            Assert.Equal("Nenhum tipo de Categoria foi selecionado!", message);
        }

        [Fact, Order(9)]
        public void Post_Returns_Bad_Request_When_TryCatch_ThrowError()
        {
            // Arrange Para ocorrer esta situação o tipo de categotia não pode ser == Todas
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var categoriaVM = CategoriaFaker.CategoriasVMs().First();
            categoriaVM.IdTipoCategoria = (int)TipoCategoria.Receita;

            SetupBearerToken(categoriaVM.IdUsuario);

            _mockCategoriaBusiness.Setup(b => b.Create(categoriaVM)).Throws(new Exception());

            // Act
            var result = _categoriaController.Post(categoriaVM) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;

            var message = value.GetType().GetProperty("message").GetValue(value, null) as string;

            Assert.Equal(
                "Não foi possível realizar o cadastro de uma nova categoria, tente mais tarde ou entre em contato com o suporte.",
                message
            );
        }

        [Fact, Order(10)]
        public void Put_Returns_Ok_Result()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var obj = CategoriaFaker.CategoriasVMs().FindAll(c => c.IdTipoCategoria != 0).First();
            var categoriaVM = new CategoriaVM
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                IdUsuario = obj.IdUsuario,
                IdTipoCategoria = (int)TipoCategoria.Despesa
            };

            SetupBearerToken(categoriaVM.IdUsuario);

            _mockCategoriaBusiness.Setup(b => b.Update(categoriaVM)).Returns(categoriaVM);

            // Act
            var result = _categoriaController.Put(categoriaVM) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value;

            var message = (bool)value.GetType().GetProperty("message").GetValue(value, null);

            Assert.True(message);

            var _categoriaVM = value.GetType().GetProperty("categoria").GetValue(value, null);

            Assert.IsType<CategoriaVM>(_categoriaVM);
            Assert.Equal(_categoriaVM, categoriaVM);
        }

        [Fact, Order(12)]
        public void Put_Returns_Bad_Request_TipoCategoria_Todas()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var categoriaVM = CategoriaFaker.CategoriasVMs().First();
            categoriaVM.IdTipoCategoria = 0;

            SetupBearerToken(categoriaVM.IdUsuario);

            _mockCategoriaBusiness.Setup(b => b.Update(categoriaVM)).Returns(categoriaVM);

            // Act
            var result = _categoriaController.Put(categoriaVM) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;
            var message = value?.GetType()?.GetProperty("message")?.GetValue(value, null) as string;
            Assert.Equal("Nenhum tipo de Categoria foi selecionado!", message);
        }

        [Fact, Order(13)]
        public void Put_Returns_Bad_Request_Categoria_Null()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var categoriaVM = CategoriaFaker.CategoriasVMs().First();
            categoriaVM.IdTipoCategoria = 1;

            SetupBearerToken(categoriaVM.IdUsuario);

            _mockCategoriaBusiness.Setup(b => b.Update(categoriaVM)).Returns((CategoriaVM)null);

            // Act
            var result = _categoriaController.Put(categoriaVM) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;

            var message = value.GetType().GetProperty("message").GetValue(value, null) as string;

            Assert.Equal("Erro ao atualizar categoria!", message);
        }

        [Fact, Order(14)]
        public void Delete_Returns_Ok_Result()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var obj = CategoriaFaker.CategoriasVMs().Last();
            var categoriaVM = new CategoriaVM
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                IdUsuario = 10,
                IdTipoCategoria = (int)TipoCategoria.Receita
            };
            SetupBearerToken(10);
            _mockCategoriaBusiness.Setup(b => b.Delete(It.IsAny<CategoriaVM>())).Returns(true);
            _mockCategoriaBusiness
                .Setup(b => b.FindById(categoriaVM.Id, categoriaVM.IdUsuario))
                .Returns(categoriaVM);

            // Act
            var result = _categoriaController.Delete(categoriaVM.Id) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var value = result.Value;

            var message = (bool)value.GetType().GetProperty("message").GetValue(value, null);

            Assert.True(message);
            _mockCategoriaBusiness.Verify(
                b => b.FindById(categoriaVM.Id, categoriaVM.IdUsuario),
                Times.Once
            );
            _mockCategoriaBusiness.Verify(b => b.Delete(categoriaVM), Times.Once);
        }

        [Fact, Order(15)]
        public void Delete_Returns_OK_Result_Message_False()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var obj = CategoriaFaker.CategoriasVMs().Last();
            var categoriaVM = new CategoriaVM
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                IdUsuario = 100,
                IdTipoCategoria = (int)TipoCategoria.Receita
            };
            SetupBearerToken(100);
            _mockCategoriaBusiness.Setup(b => b.Delete(categoriaVM)).Returns(false);
            _mockCategoriaBusiness
                .Setup(b => b.FindById(categoriaVM.Id, categoriaVM.IdUsuario))
                .Returns(categoriaVM);

            // Act
            var result = _categoriaController.Delete(categoriaVM.Id) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);
            var value = result.Value;

            var message = (bool)value.GetType().GetProperty("message").GetValue(value, null);

            Assert.False(message);
            _mockCategoriaBusiness.Verify(
                b => b.FindById(categoriaVM.Id, categoriaVM.IdUsuario),
                Times.Once
            );
            _mockCategoriaBusiness.Verify(b => b.Delete(categoriaVM), Times.Once);
        }

        [Fact, Order(16)]
        public void Delete_Returns_BadRequest()
        {
            // Arrange
            _mockCategoriaBusiness = new Mock<IBusiness<CategoriaVM>>();
            _categoriaController = new CategoriaController(_mockCategoriaBusiness.Object);
            var categoriaVM = CategoriaFaker.CategoriasVMs().First();
            SetupBearerToken(0);
            _mockCategoriaBusiness.Setup(b => b.Delete(categoriaVM)).Returns(false);

            _mockCategoriaBusiness
                .Setup(b => b.FindById(categoriaVM.Id, categoriaVM.IdUsuario))
                .Returns(categoriaVM);

            // Act
            var result = _categoriaController.Delete(categoriaVM.Id) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<BadRequestObjectResult>(result);

            _mockCategoriaBusiness.Verify(
                b => b.FindById(categoriaVM.Id, categoriaVM.IdUsuario),
                Times.Never
            );

            _mockCategoriaBusiness.Verify(b => b.Delete(categoriaVM), Times.Never);
        }
    }
}

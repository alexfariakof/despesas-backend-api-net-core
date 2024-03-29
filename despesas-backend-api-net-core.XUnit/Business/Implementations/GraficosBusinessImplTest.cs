﻿
using despesas_backend_api_net_core.Business.Implementations;
using despesas_backend_api_net_core.Infrastructure.Data.Repositories;
using Xunit.Extensions.Ordering;

namespace Business
{
    [Order(104)]
    public class GraficosBusinessImplTest
    {
        private readonly Mock<IGraficosRepositorio> _repositorioMock;
        private readonly GraficosBusinessImpl _graficosBusinessImpl;

        public GraficosBusinessImplTest()
        {
            _repositorioMock = new Mock<IGraficosRepositorio>();
            _graficosBusinessImpl = new GraficosBusinessImpl(_repositorioMock.Object);
        }

        [Fact]
        public void GetDadosGraficoByAnoByIdUsuario_Should_Return_Grafico()
        {
            // Arrange
            var idUsuario = 1;
            var data = new DateTime(2023, 10, 1);
            var graficoData = GraficoFaker.GetNewFaker();
            _repositorioMock.Setup(r => r.GetDadosGraficoByAno(idUsuario, data)).Returns(graficoData);

            // Act
            var result = _graficosBusinessImpl.GetDadosGraficoByAnoByIdUsuario(idUsuario, data);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Grafico>(result);
            _repositorioMock.Verify(r => r.GetDadosGraficoByAno(idUsuario, data), Times.Once);
        } 
    }
}
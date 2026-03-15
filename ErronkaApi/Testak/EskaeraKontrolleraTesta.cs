using ErronkaApi.DTOak;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace ErronkaApi.Testak
{
    public class EskaeraKontrolleraTesta
    {
        private readonly Mock<EskaeraRepository> _repoMock;
        private readonly EskaeraKontrollerra _controller;

        public EskaeraKontrolleraTesta()
        {
            _repoMock = new Mock<EskaeraRepository>();
            _controller = new EskaeraKontrollerra(_repoMock.Object);
        }

        // 1 - SortuEskaera
        [Fact]
        public void SortuEskaera_Ondo()
        {
            var dto = new EskaeraSortuDTO();
            _repoMock.Setup(r => r.SortuEskaera(dto))
                     .Returns((true, null, 5, null));

            var result = _controller.SortuEskaera(dto) as OkObjectResult;

            Assert.NotNull(result);
            var erantzuna = result.Value as ErantzunaDTO<int>;
            Assert.Equal(200, erantzuna.Code);
            Assert.Contains(5, erantzuna.Datuak);
        }

        [Fact]
        public void SortuEskaera_Errorea()
        {
            var dto = new EskaeraSortuDTO();
            _repoMock.Setup(r => r.SortuEskaera(dto))
                     .Returns((false, "Akatsa", null, "Xehetasunak"));

            var result = _controller.SortuEskaera(dto) as BadRequestObjectResult;

            Assert.NotNull(result);
            var erantzuna = result.Value as ErantzunaDTO<string>;
            Assert.Equal(400, erantzuna.Code);
        }

        // 2 - LortuEskaerak
        [Fact]
        public void LortuEskaerak_Ondo()
        {
            var datuak = new List<EskaeraDTO>();
            _repoMock.Setup(r => r.LortuEskaerak())
                     .Returns((true, null, datuak));

            var result = _controller.LortuEskaerak() as OkObjectResult;

            Assert.NotNull(result);
            var erantzuna = result.Value as ErantzunaDTO<EskaeraDTO>;
            Assert.Equal(200, erantzuna.Code);
        }

        [Fact]
        public void LortuEskaerak_Errorea()
        {
            _repoMock.Setup(r => r.LortuEskaerak())
                     .Returns((false, "Errorea", null));

            var result = _controller.LortuEskaerak() as ObjectResult;

            Assert.Equal(500, result.StatusCode);
        }

        // 3 - LortuEskaeraProduktuak
        [Fact]
        public void LortuEskaeraProduktuak_Ondo()
        {
            var datuak = new List<EskaeraLortuDTO>();
            _repoMock.Setup(r => r.LortuEskaeraProduktuak(1))
                     .Returns((true, null, datuak));

            var result = _controller.LortuEskaeraProduktuak(1) as OkObjectResult;

            Assert.NotNull(result);
            var erantzuna = result.Value as ErantzunaDTO<EskaeraLortuDTO>;
            Assert.Equal(200, erantzuna.Code);
        }

        [Fact]
        public void LortuEskaeraProduktuak_EzDago()
        {
            _repoMock.Setup(r => r.LortuEskaeraProduktuak(1))
                     .Returns((false, "Ez da aurkitu", null));

            var result = _controller.LortuEskaeraProduktuak(1) as NotFoundObjectResult;

            Assert.NotNull(result);
        }

        // 4 - EzabatuEskaera
        [Fact]
        public void EzabatuEskaera_Ondo()
        {
            _repoMock.Setup(r => r.EzabatuEskaera(1))
                     .Returns((true, null));

            var result = _controller.EzabatuEskaera(1) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void EzabatuEskaera_EzDago()
        {
            _repoMock.Setup(r => r.EzabatuEskaera(1))
                     .Returns((false, "Ez da aurkitu"));

            var result = _controller.EzabatuEskaera(1) as NotFoundObjectResult;

            Assert.NotNull(result);
        }

        // 5 - LortuMahaiKapazitatea
        [Fact]
        public void LortuMahaiKapazitatea_Ondo()
        {
            _repoMock.Setup(r => r.LortuMahaiKapazitatea(1))
                     .Returns((true, null, 4));

            var result = _controller.LortuMahaiKapazitatea(1) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void LortuMahaiKapazitatea_EzDago()
        {
            _repoMock.Setup(r => r.LortuMahaiKapazitatea(1))
                     .Returns((false, "Ez da aurkitu", null));

            var result = _controller.LortuMahaiKapazitatea(1) as NotFoundObjectResult;

            Assert.NotNull(result);
        }

        // 6 - EguneratuEskaera
        [Fact]
        public void EguneratuEskaera_Ondo()
        {
            var dto = new EskaeraEguneratuDTO();
            _repoMock.Setup(r => r.EguneratuEskaera(1, dto))
                     .Returns((true, null, null));

            var result = _controller.EguneratuEskaera(1, dto) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void EguneratuEskaera_Errorea()
        {
            var dto = new EskaeraEguneratuDTO();

            _repoMock.Setup(r => r.EguneratuEskaera(1, It.IsAny<EskaeraEguneratuDTO>()))
         .Returns<(bool, string, object)>((false, "Errorea", "Xehetasunak"));


            var result = _controller.EguneratuEskaera(1, dto) as BadRequestObjectResult;

            Assert.NotNull(result);
        }

        // 7 - EguneratuSukaldeaEgoera
        [Fact]
        public void EguneratuSukaldeaEgoera_Ondo()
        {
            var dto = new EskaeraSukaldeaEgoeraDTO { SukaldeaEgoera = "Prest" };
            _repoMock.Setup(r => r.EguneratuSukaldeaEgoera(1, "Prest"))
                     .Returns((true, null));

            var result = _controller.EguneratuSukaldeaEgoera(1, dto) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void EguneratuSukaldeaEgoera_Errorea()
        {
            var dto = new EskaeraSukaldeaEgoeraDTO { SukaldeaEgoera = "Prest" };
            _repoMock.Setup(r => r.EguneratuSukaldeaEgoera(1, "Prest"))
                     .Returns((false, "Errorea"));

            var result = _controller.EguneratuSukaldeaEgoera(1, dto) as BadRequestObjectResult;

            Assert.NotNull(result);
        }

        // 8 - OrdainduEskaera
        [Fact]
        public void OrdainduEskaera_Ondo()
        {
            _repoMock.Setup(r => r.OrdaintzeraBidali(1))
                     .Returns((true, null));

            var result = _controller.OrdainduEskaera(1) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void OrdainduEskaera_EzDago()
        {
            _repoMock.Setup(r => r.OrdaintzeraBidali(1))
                     .Returns((false, "Ez da aurkitu"));

            var result = _controller.OrdainduEskaera(1) as NotFoundObjectResult;

            Assert.NotNull(result);
        }

        // 9 - SortuFaktura
        [Fact]
        public void SortuFaktura_Ondo()
        {
            _repoMock.Setup(r => r.SortuFaktura(1))
                     .Returns((true, null, "faktura.pdf"));

            var result = _controller.SortuFaktura(1) as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void SortuFaktura_Errorea()
        {
            _repoMock.Setup(r => r.SortuFaktura(1))
                     .Returns((false, "Errorea", null));

            var result = _controller.SortuFaktura(1) as BadRequestObjectResult;

            Assert.NotNull(result);
        }

        // 10 - LortuEskaerakOrdaintzeko
        [Fact]
        public void LortuEskaerakOrdaintzeko_Ondo()
        {
            var datuak = new List<EskaeraDTO>();
            _repoMock.Setup(r => r.LortuEskaerakOrdaintzeko())
                     .Returns((true, null, datuak));

            var result = _controller.LortuEskaerakOrdaintzeko() as OkObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void LortuEskaerakOrdaintzeko_Errorea()
        {
            _repoMock.Setup(r => r.LortuEskaerakOrdaintzeko())
                     .Returns((false, "Errorea", null));

            var result = _controller.LortuEskaerakOrdaintzeko() as ObjectResult;

            Assert.Equal(500, result.StatusCode);
        }
    }
}

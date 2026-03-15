using ErronkaApi.DTOak;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace ErronkaApi.Testak
{
    public class EskaeraKontrollerraTesta
    {
        private readonly Mock<EskaeraRepository> _repoMock;
        private readonly EskaeraKontrollerra _controller;

        public EskaeraKontrollerraTesta()
        {
            _repoMock = new Mock<EskaeraRepository>();
            _controller = new EskaeraKontrollerra(_repoMock.Object);
        }

        // SortuEskaera([FromBody] EskaeraSortuDTO dto)

        [Fact]
        public void SortuEskaera_BadRequestItzultzenDu_RepositorioakErroreaEmatenDuenean()
        {
            var dto = new EskaeraSortuDTO();

            _repoMock.Setup(r => r.SortuEskaera(dto))
                .Returns((false, "Errorea", null, (List<string>)null));

            var result = _controller.SortuEskaera(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void SortuEskaera_OkItzultzenDu_EskaeraOndoSortzenDenean()
        {
            var dto = new EskaeraSortuDTO();

            _repoMock.Setup(r => r.SortuEskaera(dto))
                .Returns((true, null, 1, null));

            var result = _controller.SortuEskaera(dto);

            Assert.IsType<OkObjectResult>(result);
        }

        // LortuEskaerak()

        [Fact]
        public void LortuEskaerak_Status500ItzultzenDu_RepositorioakErroreaEmatenDuenean()
        {
            _repoMock.Setup(r => r.LortuEskaerak())
                .Returns((false, "Errorea", null));

            var result = _controller.LortuEskaerak();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void LortuEskaerak_OkItzultzenDu_EskaerakOndoLortzenDirenean()
        {
            var lista = new List<EskaeraDTO>();

            _repoMock.Setup(r => r.LortuEskaerak())
                .Returns((true, null, lista));

            var result = _controller.LortuEskaerak();

            Assert.IsType<OkObjectResult>(result);
        }

        // LortuEskaeraProduktuak(int eskaeraId)

        [Fact]
        public void LortuEskaeraProduktuak_NotFoundItzultzenDu_EskaeraExistitzenEzDenean()
        {
            _repoMock.Setup(r => r.LortuEskaeraProduktuak(1))
                .Returns((false, "Ez da aurkitu", null));

            var result = _controller.LortuEskaeraProduktuak(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void LortuEskaeraProduktuak_OkItzultzenDu_ProduktuakOndoLortzenDirenean()
        {
            var lista = new List<EskaeraLortuDTO>();

            _repoMock.Setup(r => r.LortuEskaeraProduktuak(1))
                .Returns((true, null, lista));

            var result = _controller.LortuEskaeraProduktuak(1);

            Assert.IsType<OkObjectResult>(result);
        }

        // EzabatuEskaera(int eskaeraId)

        [Fact]
        public void EzabatuEskaera_NotFoundItzultzenDu_EskaeraExistitzenEzDenean()
        {
            _repoMock.Setup(r => r.EzabatuEskaera(1))
                .Returns((false, "Ez da aurkitu"));

            var result = _controller.EzabatuEskaera(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void EzabatuEskaera_OkItzultzenDu_EskaeraOndoEzabatzenDenean()
        {
            _repoMock.Setup(r => r.EzabatuEskaera(1))
                .Returns((true, null));

            var result = _controller.EzabatuEskaera(1);

            Assert.IsType<OkObjectResult>(result);
        }

        // LortuMahaiKapazitatea(int mahaiaId)

        [Fact]
        public void LortuMahaiKapazitatea_NotFoundItzultzenDu_MahaiaExistitzenEzDenean()
        {
            _repoMock.Setup(r => r.LortuMahaiKapazitatea(1))
                .Returns((false, "Ez da aurkitu", null));

            var result = _controller.LortuMahaiKapazitatea(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void LortuMahaiKapazitatea_OkItzultzenDu_KapazitateaOndoLortzenDenean()
        {
            _repoMock.Setup(r => r.LortuMahaiKapazitatea(1))
                .Returns((true, null, 4));

            var result = _controller.LortuMahaiKapazitatea(1);

            Assert.IsType<OkObjectResult>(result);
        }

        // EguneratuEskaera(int eskaeraId, [FromBody] EskaeraEguneratuDTO dto)

        [Fact]
        public void EguneratuEskaera_BadRequestItzultzenDu_RepositorioakErroreaEmatenDuenean()
        {
            var dto = new EskaeraEguneratuDTO();

            _repoMock.Setup(r => r.EguneratuEskaera(1, dto))
                .Returns((false, "Errorea", null));

            var result = _controller.EguneratuEskaera(1, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void EguneratuEskaera_OkItzultzenDu_EskaeraOndoEguneratzenDenean()
        {
            var dto = new EskaeraEguneratuDTO();

            _repoMock.Setup(r => r.EguneratuEskaera(1, dto))
                .Returns((true, null, null));

            var result = _controller.EguneratuEskaera(1, dto);

            Assert.IsType<OkObjectResult>(result);
        }

        // EguneratuSukaldeaEgoera(int eskaeraId, [FromBody] EskaeraSukaldeaEgoeraDTO dto)

        [Fact]
        public void EguneratuSukaldeaEgoera_BadRequestItzultzenDu_RepositorioakErroreaEmatenDuenean()
        {
            var dto = new EskaeraSukaldeaEgoeraDTO { SukaldeaEgoera = "prest" };

            _repoMock.Setup(r => r.EguneratuSukaldeaEgoera(1, "prest"))
                .Returns((false, "Errorea"));

            var result = _controller.EguneratuSukaldeaEgoera(1, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void EguneratuSukaldeaEgoera_OkItzultzenDu_EgoeraOndoEguneratzenDenean()
        {
            var dto = new EskaeraSukaldeaEgoeraDTO { SukaldeaEgoera = "prest" };

            _repoMock.Setup(r => r.EguneratuSukaldeaEgoera(1, "prest"))
                .Returns((true, null));

            var result = _controller.EguneratuSukaldeaEgoera(1, dto);

            Assert.IsType<OkObjectResult>(result);
        }

        // OrdainduEskaera(int eskaeraId)

        [Fact]
        public void OrdainduEskaera_NotFoundItzultzenDu_EskaeraExistitzenEzDenean()
        {
            _repoMock.Setup(r => r.OrdaintzeraBidali(1))
                .Returns((false, "Ez da aurkitu"));

            var result = _controller.OrdainduEskaera(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void OrdainduEskaera_OkItzultzenDu_EskaeraOrdainketaraBidaltzenDenean()
        {
            _repoMock.Setup(r => r.OrdaintzeraBidali(1))
                .Returns((true, null));

            var result = _controller.OrdainduEskaera(1);

            Assert.IsType<OkObjectResult>(result);
        }

        // SortuFaktura(int eskaeraId)

        [Fact]
        public void SortuFaktura_BadRequestItzultzenDu_FakturaSortzeanErroreaGertatzenDenean()
        {
            _repoMock.Setup(r => r.SortuFaktura(1))
                .Returns((false, "Errorea", null));

            var result = _controller.SortuFaktura(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void SortuFaktura_OkItzultzenDu_FakturaOndoSortzenDenean()
        {
            _repoMock.Setup(r => r.SortuFaktura(1))
                .Returns((true, null, "faktura.pdf"));

            var result = _controller.SortuFaktura(1);

            Assert.IsType<OkObjectResult>(result);
        }

        // LortuEskaerakOrdaintzeko()

        [Fact]
        public void LortuEskaerakOrdaintzeko_Status500ItzultzenDu_RepositorioakErroreaEmatenDuenean()
        {
            _repoMock.Setup(r => r.LortuEskaerakOrdaintzeko())
                .Returns((false, "Errorea", null));

            var result = _controller.LortuEskaerakOrdaintzeko();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void LortuEskaerakOrdaintzeko_OkItzultzenDu_EskaerakOndoLortzenDirenean()
        {
            var lista = new List<EskaeraDTO>();

            _repoMock.Setup(r => r.LortuEskaerakOrdaintzeko())
                .Returns((true, null, lista));

            var result = _controller.LortuEskaerakOrdaintzeko();

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
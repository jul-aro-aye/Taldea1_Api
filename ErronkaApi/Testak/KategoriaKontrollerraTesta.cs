using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using System.Collections.Generic;

namespace ErronkaApi.Kontrollerrak
{
    public class KategoriaKontrollerraTesta
    {
        // LortuKategoriak()

        [Fact]
        public void LortuKategoriak_Status500ItzultzenDu_RepositorioakErroreaEmatenDuenean()
        {
            var repoMock = new Mock<KategoriaRepository>();

            repoMock.Setup(r => r.LortuKategoriak())
                .Returns((false, "errorea", null));

            var controller = new KategoriaKontrollerra(repoMock.Object);

            var result = controller.LortuKategoriak();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void LortuKategoriak_OkItzultzenDu_KategoriakOndoLortzenDirenean()
        {
            var repoMock = new Mock<KategoriaRepository>();

            var lista = new List<KategoriaDTO>
            {
                new KategoriaDTO { id = 1, izena = "TestKategoria" }
            };

            repoMock.Setup(r => r.LortuKategoriak())
                .Returns((true, null, lista));

            var controller = new KategoriaKontrollerra(repoMock.Object);

            var result = controller.LortuKategoriak();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        // LortuKategoria(int id)

        [Fact]
        public void LortuKategoria_NotFoundItzultzenDu_KategoriaExistitzenEzDenean()
        {
            var repoMock = new Mock<KategoriaRepository>();

            repoMock.Setup(r => r.LortuKategoria(1))
                .Returns((false, "Kategoria ez da aurkitu", null));

            var controller = new KategoriaKontrollerra(repoMock.Object);

            var result = controller.LortuKategoria(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void LortuKategoria_OkItzultzenDu_KategoriaExistitzenDenean()
        {
            var repoMock = new Mock<KategoriaRepository>();

            var kategoria = new KategoriaDTO
            {
                id = 1,
                izena = "TestKategoria"
            };

            repoMock.Setup(r => r.LortuKategoria(1))
                .Returns((true, null, kategoria));

            var controller = new KategoriaKontrollerra(repoMock.Object);

            var result = controller.LortuKategoria(1);

            Assert.IsType<OkObjectResult>(result);
        }

        // GehituKategoria()

        [Fact]
        public void GehituKategoria_BadRequestItzultzenDu_DatuakBaliozkoakEzDirenean()
        {
            var repoMock = new Mock<KategoriaRepository>();
            var dto = new KategoriaDTO();

            repoMock.Setup(r => r.GehituKategoria(dto))
                .Returns((false, "errorea"));

            var controller = new KategoriaKontrollerra(repoMock.Object);

            var result = controller.GehituKategoria(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GehituKategoria_OkItzultzenDu_KategoriaOndoGehitzenDenean()
        {
            var repoMock = new Mock<KategoriaRepository>();
            var dto = new KategoriaDTO();

            repoMock.Setup(r => r.GehituKategoria(dto))
                .Returns((true, null));

            var controller = new KategoriaKontrollerra(repoMock.Object);

            var result = controller.GehituKategoria(dto);

            Assert.IsType<OkObjectResult>(result);
        }

        // EguneratuKategoria()

        [Fact]
        public void EguneratuKategoria_NotFoundItzultzenDu_KategoriaExistitzenEzDenean()
        {
            var repoMock = new Mock<KategoriaRepository>();
            var dto = new KategoriaDTO();

            repoMock.Setup(r => r.EguneratuKategoria(1, dto))
                .Returns((false, "Kategoria ez da aurkitu"));

            var controller = new KategoriaKontrollerra(repoMock.Object);

            var result = controller.EguneratuKategoria(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void EguneratuKategoria_OkItzultzenDu_KategoriaOndoEguneratzenDenean()
        {
            var repoMock = new Mock<KategoriaRepository>();
            var dto = new KategoriaDTO();

            repoMock.Setup(r => r.EguneratuKategoria(1, dto))
                .Returns((true, null));

            var controller = new KategoriaKontrollerra(repoMock.Object);

            var result = controller.EguneratuKategoria(1, dto);

            Assert.IsType<OkObjectResult>(result);
        }

        // EzabatuKategoria()

        [Fact]
        public void EzabatuKategoria_NotFoundItzultzenDu_KategoriaExistitzenEzDenean()
        {
            var repoMock = new Mock<KategoriaRepository>();

            repoMock.Setup(r => r.EzabatuKategoria(1))
                .Returns((false, "Kategoria ez da aurkitu"));

            var controller = new KategoriaKontrollerra(repoMock.Object);

            var result = controller.EzabatuKategoria(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void EzabatuKategoria_OkItzultzenDu_KategoriaOndoEzabatzenDenean()
        {
            var repoMock = new Mock<KategoriaRepository>();

            repoMock.Setup(r => r.EzabatuKategoria(1))
                .Returns((true, null));

            var controller = new KategoriaKontrollerra(repoMock.Object);

            var result = controller.EzabatuKategoria(1);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
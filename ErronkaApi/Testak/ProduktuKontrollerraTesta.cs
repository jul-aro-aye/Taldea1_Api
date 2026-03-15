using ErronkaApi.DTOak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace ErronkaApi.Kontrollerrak
{
    public class ProduktuKontrollerraTesta
    {

        // LortuProduktuak()

        [Fact]
        public void LortuProduktuak_Status500ItzultzenDu_RepoakErroreaEmatenDuenean()
        {
            var repoMock = new Mock<ProduktuaRepository>();
            repoMock.Setup(r => r.LortuProduktuak())
                .Returns((false, "errorea", null));

            var controller = new ProduktuakKontrollera(repoMock.Object);

            var result = controller.LortuProduktuak();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void LortuProduktuak_OkItzultzenDu_ProduktuakOndoLortzenDirenean()
        {
            var repoMock = new Mock<ProduktuaRepository>();

            var lista = new List<ProduktuaDTO>
            {
                new ProduktuaDTO { id = 1, izena = "Test", prezioa = 10, kategoria_id = 1, stock_aktuala = 5 }
            };

            repoMock.Setup(r => r.LortuProduktuak())
                .Returns((true, null, lista));

            var controller = new ProduktuakKontrollera(repoMock.Object);

            var result = controller.LortuProduktuak();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        // LortuProduktua()

        [Fact]
        public void LortuProduktua_NotFoundItzultzenDu_ProduktuaExistitzenEzDenean()
        {
            var repoMock = new Mock<ProduktuaRepository>();

            repoMock.Setup(r => r.LortuProduktua(1))
                .Returns((false, "Produktua ez da aurkitu", null));

            var controller = new ProduktuakKontrollera(repoMock.Object);

            var result = controller.LortuProduktua(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void LortuProduktua_OkItzultzenDu_ProduktuaExistitzenDenean()
        {
            var repoMock = new Mock<ProduktuaRepository>();

            var produktua = new ProduktuaDTO
            {
                id = 1,
                izena = "Test",
                prezioa = 10,
                kategoria_id = 1,
                stock_aktuala = 3
            };

            repoMock.Setup(r => r.LortuProduktua(1))
                .Returns((true, null, produktua));

            var controller = new ProduktuakKontrollera(repoMock.Object);

            var result = controller.LortuProduktua(1);

            Assert.IsType<OkObjectResult>(result);
        }

        // GehituProduktua()

        [Fact]
        public void GehituProduktua_BadRequestItzultzenDu_DatuakBaliozkoakEzDirenean()
        {
            var repoMock = new Mock<ProduktuaRepository>();
            var dto = new ProduktuaDTO();

            repoMock.Setup(r => r.GehituProduktua(dto))
                .Returns((false, "errorea"));

            var controller = new ProduktuakKontrollera(repoMock.Object);

            var result = controller.GehituProduktua(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void GehituProduktua_OkItzultzenDu_ProduktuaOndoGehitzenDenean()
        {
            var repoMock = new Mock<ProduktuaRepository>();
            var dto = new ProduktuaDTO();

            repoMock.Setup(r => r.GehituProduktua(dto))
                .Returns((true, null));

            var controller = new ProduktuakKontrollera(repoMock.Object);

            var result = controller.GehituProduktua(dto);

            Assert.IsType<OkObjectResult>(result);
        }

        // EguneratuProduktua()

        [Fact]
        public void EguneratuProduktua_NotFoundItzultzenDu_ProduktuaExistitzenEzDenean()
        {
            var repoMock = new Mock<ProduktuaRepository>();
            var dto = new ProduktuaDTO();

            repoMock.Setup(r => r.EguneratuProduktua(1, dto))
                .Returns((false, "Produktua ez da aurkitu"));

            var controller = new ProduktuakKontrollera(repoMock.Object);

            var result = controller.EguneratuProduktua(1, dto);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void EguneratuProduktua_OkItzultzenDu_ProduktuaOndoEguneratzenDenean()
        {
            var repoMock = new Mock<ProduktuaRepository>();
            var dto = new ProduktuaDTO();

            repoMock.Setup(r => r.EguneratuProduktua(1, dto))
                .Returns((true, null));

            var controller = new ProduktuakKontrollera(repoMock.Object);

            var result = controller.EguneratuProduktua(1, dto);

            Assert.IsType<OkObjectResult>(result);
        }

        // EzabatuProduktua()

        [Fact]
        public void EzabatuProduktua_NotFoundItzultzenDu_ProduktuaExistitzenEzDenean()
        {
            var repoMock = new Mock<ProduktuaRepository>();

            repoMock.Setup(r => r.EzabatuProduktua(1))
                .Returns((false, "Produktua ez da aurkitu"));

            var controller = new ProduktuakKontrollera(repoMock.Object);

            var result = controller.EzabatuProduktua(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void EzabatuProduktua_OkItzultzenDu_ProduktuaOndoEzabatzenDenean()
        {
            var repoMock = new Mock<ProduktuaRepository>();

            repoMock.Setup(r => r.EzabatuProduktua(1))
                .Returns((true, null));

            var controller = new ProduktuakKontrollera(repoMock.Object);

            var result = controller.EzabatuProduktua(1);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
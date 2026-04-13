using ErronkaApi.DTOak;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ErronkaApi.Testak
{
    public class MahaiaKontrollerTestak
    {
        // Testak LortuMahaiLibre metodoarentzat
        [Fact]
        public void LortuMahaiLibre_ErroreaGertatzenBada_500ItzuliBeharDu()
        {
            // Arrange
            var mockRepo = new Mock<MahaiaRepository>();
            mockRepo.Setup(r => r.LortuMahaiLibre(null, null))
                    .Returns((false, "Errorea gertatu da", null as List<MahaiaDTO>));

            var controller = new MahaiakController(mockRepo.Object);

            // Act
            var result = controller.LortuMahaiLibre();

            // Assert
            var status = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, status.StatusCode);

            var body = Assert.IsType<ErantzunaDTO<string>>(status.Value);
            Assert.Equal(500, body.Code);
            Assert.Equal("Errorea gertatu da", body.Message);
        }


        [Fact]
        public void LortuMahaiLibre_DaturikEzBadago_404ItzuliBeharDu()
        {
            // Arrange
            var mockRepo = new Mock<MahaiaRepository>();
            mockRepo.Setup(r => r.LortuMahaiLibre(null, null))
                    .Returns((true, null, new List<MahaiaDTO>()));

            var controller = new MahaiakController(mockRepo.Object);

            // Act
            var result = controller.LortuMahaiLibre();

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            var body = Assert.IsType<ErantzunaDTO<string>>(notFound.Value);
            Assert.Equal(404, body.Code);
            Assert.Equal("Ez dago mahai librerik", body.Message);
        }

        [Fact]
        public void LortuMahaiLibre_DatuakDaudenean_200EtaMahaienZerrendaItzuliBeharDu()
        {
            // Arrange
            var data = new List<MahaiaDTO>
            {
                new MahaiaDTO
                {
                    Id = 1,
                    Zenbakia = 5,
                    kapazitatea = 4
                }
            };

            var mockRepo = new Mock<MahaiaRepository>();
            mockRepo.Setup(r => r.LortuMahaiLibre(null, null))
                    .Returns((true, null, data));

            var controller = new MahaiakController(mockRepo.Object);

            // Act
            var result = controller.LortuMahaiLibre();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);

            var body = Assert.IsType<ErantzunaDTO<MahaiaDTO>>(ok.Value);
            Assert.Equal(200, body.Code);
            Assert.Equal("Mahai libreak lortu dira", body.Message);

            var lista = body.Datuak;
            Assert.Single(lista);

            var mahaia = lista.First();
            Assert.Equal(1, mahaia.Id);
            Assert.Equal(5, mahaia.Zenbakia);
            Assert.Equal(4, mahaia.kapazitatea);
        }


        // Testak LortuMahaiBat metodoarentzat
        [Fact]
        public void LortuMahaiBat_EzBadaExistitzen_404ItzuliBeharDu()
        {
            // Arrange
            var mockRepo = new Mock<MahaiaRepository>();
            mockRepo.Setup(r => r.LortuMahaiBat(5, null, null))
                    .Returns((false, "Mahaia ez da existitzen", null as MahaiaDTO));

            var controller = new MahaiakController(mockRepo.Object);

            // Act
            var result = controller.LortuMahaiBat(5);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);

            var body = Assert.IsType<ErantzunaDTO<string>>(notFound.Value);
            Assert.Equal(404, body.Code);
            Assert.Equal("Mahaia ez da existitzen", body.Message);
        }

        [Fact]
        public void LortuMahaiBat_DatuZuzenekin_200EtaMahaiaItzuliBeharDu()
        {
            // Arrange
            var mahaia = new MahaiaDTO
            {
                Id = 1,
                Zenbakia = 10,
                kapazitatea = 4
            };

            var mockRepo = new Mock<MahaiaRepository>();
            mockRepo.Setup(r => r.LortuMahaiBat(1, null, null))
                    .Returns((true, null, mahaia));

            var controller = new MahaiakController(mockRepo.Object);

            // Act
            var result = controller.LortuMahaiBat(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);

            var body = Assert.IsType<ErantzunaDTO<MahaiaDTO>>(ok.Value);
            Assert.Equal(200, body.Code);
            Assert.Equal("Mahaia lortu da", body.Message);

            var lista = body.Datuak;
            Assert.Single(lista);

            var m = lista.First();
            Assert.Equal(1, m.Id);
            Assert.Equal(10, m.Zenbakia);
            Assert.Equal(4, m.kapazitatea);
        }
    }
}

using ErronkaApi.DTOak;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Modeloak;
using ErronkaApi.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ErronkaApi.Testak
{
    public class ErabiltzaileaKontrollerraTestak
    {
        [Fact]
        public void LortuLoginErabiltzaileak_ErroreaBadago_BadRequestItzuliBeharDu()
        {
            // Arrange
            var mockRepo = new Mock<ErabiltzaileaRepository>();
            mockRepo.Setup(r => r.LortuAktiboak())
                    .Returns((false, "DB errorea", null as List<Erabiltzailea>));

            var controller = new ErabiltzaileaKontrollerra(mockRepo.Object);

            // Act
            var result = controller.LortuLoginErabiltzaileak();

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var body = Assert.IsType<ErantzunaDTO<string>>(badRequest.Value);

            Assert.Equal(400, body.Code);
            Assert.Equal("DB errorea", body.Message);
            Assert.Null(body.Datuak);
        }

        [Fact]
        public void LortuLoginErabiltzaileak_DatuakDaudenean_OkEtaErabiltzaileakItzuliBeharDitu()
        {
            // Arrange
            var erabiltzaileak = new List<Erabiltzailea>
            {
                new Erabiltzailea
                {
                    id = 1,
                    erabiltzailea = "ane",
                    emaila = "ane@test.com",
                    rola = new Rola { id = 2, izena = "zerbitzaria" },
                    txat = true
                }
            };

            var mockRepo = new Mock<ErabiltzaileaRepository>();
            mockRepo.Setup(r => r.LortuAktiboak())
                    .Returns((true, null, erabiltzaileak));

            var controller = new ErabiltzaileaKontrollerra(mockRepo.Object);

            // Act
            var result = controller.LortuLoginErabiltzaileak();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var body = Assert.IsType<ErantzunaDTO<ErabiltzaileaLoginDTO>>(ok.Value);

            Assert.Equal(200, body.Code);
            Assert.Equal("Erabiltzaileak zuzen lortu dira", body.Message);
            Assert.NotNull(body.Datuak);
            Assert.Single(body.Datuak);

            var erabiltzailea = body.Datuak!.First();
            Assert.Equal(1, erabiltzailea.id);
            Assert.Equal("ane", erabiltzailea.erabiltzailea);
            Assert.Equal("ane@test.com", erabiltzailea.emaila);
            Assert.Equal(2, erabiltzailea.rolaId);
            Assert.Equal("zerbitzaria", erabiltzailea.rolaIzena);
            Assert.True(erabiltzailea.txat);
        }

        [Fact]
        public void SortuZerbitzaria_ErroreaBadago_BadRequestItzuliBeharDu()
        {
            // Arrange
            var eskaera = new ErabiltzaileaSortuDTO
            {
                erabiltzailea = "ane",
                emaila = "ane@test.com",
                pasahitza = "sekretua",
                txat = true
            };

            var mockRepo = new Mock<ErabiltzaileaRepository>();
            mockRepo.Setup(r => r.SortuZerbitzaria("ane", "ane@test.com", "sekretua", true))
                    .Returns((false, "Errorea zerbitzaria sortzean", null as Erabiltzailea));

            var controller = new ErabiltzaileaKontrollerra(mockRepo.Object);

            // Act
            var result = controller.SortuZerbitzaria(eskaera);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var body = Assert.IsType<ErantzunaDTO<string>>(badRequest.Value);

            Assert.Equal(400, body.Code);
            Assert.Equal("Errorea zerbitzaria sortzean", body.Message);
            Assert.Null(body.Datuak);
        }

        [Fact]
        public void SortuZerbitzaria_DatuZuzenekin_OkEtaZerbitzariaItzuliBeharDu()
        {
            // Arrange
            var eskaera = new ErabiltzaileaSortuDTO
            {
                erabiltzailea = "ane",
                emaila = "ane@test.com",
                pasahitza = "sekretua",
                txat = true
            };

            var user = new Erabiltzailea
            {
                id = 7,
                erabiltzailea = "ane",
                emaila = "ane@test.com",
                rola = new Rola { id = 2, izena = "zerbitzaria" },
                txat = true
            };

            var mockRepo = new Mock<ErabiltzaileaRepository>();
            mockRepo.Setup(r => r.SortuZerbitzaria("ane", "ane@test.com", "sekretua", true))
                    .Returns((true, null, user));

            var controller = new ErabiltzaileaKontrollerra(mockRepo.Object);

            // Act
            var result = controller.SortuZerbitzaria(eskaera);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var body = Assert.IsType<ErantzunaDTO<ErabiltzaileaLoginDTO>>(ok.Value);

            Assert.Equal(200, body.Code);
            Assert.Equal("Zerbitzaria zuzen sortu da", body.Message);
            Assert.NotNull(body.Datuak);
            Assert.Single(body.Datuak);

            var sortua = body.Datuak!.First();
            Assert.Equal(7, sortua.id);
            Assert.Equal("ane", sortua.erabiltzailea);
            Assert.Equal("ane@test.com", sortua.emaila);
            Assert.Equal(2, sortua.rolaId);
            Assert.Equal("zerbitzaria", sortua.rolaIzena);
            Assert.True(sortua.txat);
        }
    }
}

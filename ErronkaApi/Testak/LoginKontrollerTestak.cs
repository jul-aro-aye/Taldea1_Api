using ErronkaApi.DTOak;
using ErronkaApi.Modeloak;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using ErronkaApi.Kontrollerrak;
using ErronkaApi.Repositorioak;

namespace ErronkaApi.Testak
{
    
    using System;
    public class LoginKontrollerTestak
    {

        [Fact]
        public void Login_DatuZuzenekin_OkEtaErabiltzailearenDatuakItzuliBeharDitu()
        {
            // Arrange
            var erabiltzailea = new Erabiltzailea
            {
                id = 1,
                erabiltzailea = "jon",
                emaila = "jon@test.com",
                pasahitza = "1234",
                rola = new Rola { id = 1, izena = "admin" },
                ezabatua = false,
                txat = true
            };

            var mockRepo = new Mock<ErabiltzaileaRepository>();

            mockRepo.Setup(r => r.Login("jon", "1234"))
                    .Returns((true, null, erabiltzailea));

            var controller = new LoginKontrollera(mockRepo.Object);

            var dto = new LoginDTO
            {
                erabiltzailea = "jon",
                pasahitza = "1234"
            };

            // Act
            var result = controller.Login(dto);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var body = Assert.IsType<ErantzunaDTO<object>>(ok.Value);

            Assert.Equal(200, body.Code);
            Assert.Equal("Login egokia", body.Message);

            var lista = Assert.IsType<List<object>>(body.Datuak);
            Assert.Single(lista);

            dynamic datuak = lista[0];

            Assert.Equal(1, datuak.id);
            Assert.Equal("jon", datuak.erabiltzailea);
            Assert.Equal("jon@test.com", datuak.emaila);
            Assert.Equal("admin", datuak.rola.izena);
            Assert.True(datuak.txat);
        }

        [Fact]
        public void Login_ErabiltzaileEdoPasahitzOkerrarekin_UnauthorizedItzuliBeharDu()
        {
            // Arrange
            var mockRepo = new Mock<ErabiltzaileaRepository>();

            mockRepo.Setup(r => r.Login("erabiltzailea", "Gaizki"))
                    .Returns((false, "Erabiltzailea edo pasahitza okerra", null as Erabiltzailea));

            var controller = new LoginKontrollera(mockRepo.Object);

            var dto = new LoginDTO
            {
                erabiltzailea = "erabiltzailea",
                pasahitza = "Gaizki"
            };

            // Act
            var result = controller.Login(dto);

            // Assert
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            var body = Assert.IsType<ErantzunaDTO<string>>(unauthorized.Value);

            Assert.Equal(401, body.Code);
            Assert.Equal("Erabiltzailea edo pasahitza okerra", body.Message);
            Assert.Null(body.Datuak);
        }
    }
}

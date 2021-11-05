using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Regnbuelinja.Controllers;
using Regnbuelinja.DAL;
using Regnbuelinja.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Regnbuelinja_Test
{
    public class AdminTest
    {
        private const string _loggetInn = "loggetInn";
        private const string _ikkeLoggetInn = "";
        private const string _brukernavn = "brukernavn";

        private readonly Mock<IBestillingRepository> mockRep = new Mock<IBestillingRepository>();
        private readonly Mock<ILogger<AdminController>> mockLog = new Mock<ILogger<AdminController>>();
        private readonly Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
        private readonly MockHttpSession mockSession = new MockHttpSession();

        [Fact]
        public async Task LoggInnFeilBrukerinput()
        {
            //Arrange
            mockRep.Setup(b => b.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(It.Is<int>(i => i != 0));

            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("Brukernavn", "Feil i inputvalidering på server");

            //Act
            var resultat = await adminController.LoggInn(It.IsAny<Bruker>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalideringen på server", resultat.Value);
        }

        [Fact]
        public async Task LoggInnFeilBrukernavnEllerPassord()
        {
            //Arrange
            mockRep.Setup(b => b.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(0);

            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LoggInn(It.IsAny<Bruker>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.False((bool)resultat.Value);
        }


        [Fact]
        public void LoggUtOk()
        {
            //Arrange
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            adminController.LoggUt("/admin/login");

            //Assert
            Assert.Equal(mockSession[_loggetInn], _ikkeLoggetInn);
        }

        [Fact]
        public async Task LagreBrukerLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBruker(It.IsAny<Bruker>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBruker(It.IsAny<Bruker>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task LagreBrukerLoggetInnFeilInput()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBruker(It.IsAny<Bruker>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("Brukernavn", "Feil i inputvalidering på server.");

            //Act
            var resultat = await adminController.LagreBruker(It.IsAny<Bruker>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering på server", resultat.Value);
        }

        [Fact]
        public async Task LagreBrukerLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBruker(It.IsAny<Bruker>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBruker(It.IsAny<Bruker>()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
            Assert.Equal("Databasefeil. Bruker ikke opprettet", resultat.Value);
        }


        [Fact]
        public async Task LagreBrukerIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBruker(It.IsAny<Bruker>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBruker(It.IsAny<Bruker>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }


        [Fact]
        public async Task HentAlleRuterLoggetInnOK()
        {
            //Arrange
            var rute1 = new Rute { Id = 1, Startpunkt = "Haiti", Endepunkt = "Oslo", Pris = 9999.99 };
            var rute2 = new Rute { Id = 2, Startpunkt = "Stockholm", Endepunkt = "Fredrikshavn", Pris = 499.99 };

            var ruteListe = new List<Rute>();
            ruteListe.Add(rute1);
            ruteListe.Add(rute2);

            mockRep.Setup(b => b.HentAlleRuter()).ReturnsAsync(ruteListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleRuter() as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Rute>)resultat.Value, ruteListe);
        }

        [Fact]
        public async Task HentAlleRuterIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentAlleRuter()).ReturnsAsync(It.IsAny<List<Rute>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleRuter() as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentAlleRuterFeilDb()
        {
            mockRep.Setup(b => b.HentAlleRuter()).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleRuter() as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
            Assert.Equal("Databasefeil. Ruter ikke hentet", resultat.Value);
        }

        [Fact]
        public async Task HentEnRuteLoggetInnOk()
        {
            //Arrange
            var rute = new Rute
            {
                Id = 1,
                Startpunkt = "Stockholm",
                Endepunkt = "Oslo",
                Pris = 798.99
            };

            mockRep.Setup(b => b.HentEnRute(It.IsAny<int>())).ReturnsAsync(rute);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnRute(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<Rute>((Rute)resultat.Value, rute);
        }

        [Fact]
        public async Task HentEnRuteIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnRute(It.IsAny<int>())).ReturnsAsync(It.IsAny<Rute>());
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnRute(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentEnRuteLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnRute(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnRute(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Ingen rute funnet", resultat.Value);
        }

        [Fact]
        public async Task LagreRuteLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.LagreRute(It.IsAny<Ruter>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreRute(It.IsAny<Ruter>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task LagreRuteLoggetInnFeilInput()
        {
            //Arrange
            mockRep.Setup(b => b.LagreRute(It.IsAny<Ruter>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("Avreisehavn", "Feil i inputvalidering på server.");

            //Act
            var resultat = await adminController.LagreRute(It.IsAny<Ruter>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering på server.", resultat.Value);
        }

        [Fact]
        public async Task LagreRuteLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.LagreRute(It.IsAny<Ruter>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreRute(It.IsAny<Ruter>()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
            Assert.Equal("Databasefeil. Rute ikke lagret", resultat.Value);
        }


        [Fact]
        public async Task LagreRuteIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.LagreRute(It.IsAny<Ruter>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreRute(It.IsAny<Ruter>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettRuteIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.SlettRute(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettRute(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettRuteLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.SlettRute(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettRute(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }


        // Vet ikke om testmiljøet blir riktig. Sjekk dette om igjen
        [Fact]
        public async Task SlettRuteLoggetInnIkkeOk()
        {
            //Arrange
            mockRep.Setup(b => b.SlettRute(It.IsAny<int>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettRute(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Rute ikke funnet eller rute med i eksisterende bestilling(er).", resultat.Value);
        }

        [Fact]
        public async Task EndreRuteIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.EndreRute(It.IsAny<Ruter>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreRute(It.IsAny<Ruter>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task EndreRuteLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.EndreRute(It.IsAny<Ruter>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreRute(It.IsAny<Ruter>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }


        
        [Fact]
        public async Task EndreRuteLoggetInnIkkeOK()
        {
            //Arrange
           
            mockRep.Setup(b => b.EndreRute(It.IsAny<Ruter>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreRute(It.IsAny<Ruter>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Rute ikke funnet eller rute med i eksisterende bestilling(er).", resultat.Value);
        }

        [Fact]
        public async Task EndreRuteLoggetInnFeilInput()
        {
            //Arrange
            mockRep.Setup(b => b.EndreRute(It.IsAny<Ruter>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("Avreisehavn", "Feil i inputvalidering på server.");

            //Act
            var resultat = await adminController.EndreRute(It.IsAny<Ruter>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering på server.", resultat.Value);
        }
        [Fact]
        public async Task HentAlleBåterLoggetInnOK()
        {
            //Arrange
            var båt1 = new Baat { Id = 1, Navn = "Kontiki" };
            var båt2 = new Baat { Id = 2, Navn = "HM Queen Victoria" };

            var båtListe = new List<Baat>();
            båtListe.Add(båt1);
            båtListe.Add(båt2);

            mockRep.Setup(b => b.HentAlleBåter()).ReturnsAsync(båtListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleBåter() as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Baat>)resultat.Value, båtListe);
        }

        [Fact]
        public async Task HentAlleBåterIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentAlleBåter()).ReturnsAsync(It.IsAny<List<Baat>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleBåter() as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentAlleBåterFeilDb()
        {
            mockRep.Setup(b => b.HentAlleBåter()).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleBåter() as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
            Assert.Equal("Databasefeil. Båter ikke hentet", resultat.Value);
        }

        [Fact]
        public async Task HentEnBåtLoggetInnOk()
        {
            //Arrange
            var båt = new Baat
            {
                Id = 1,
                Navn = "Kontiki"
            };

            mockRep.Setup(b => b.HentEnBåt(It.IsAny<int>())).ReturnsAsync(båt);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnBåt(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<Baat>((Baat)resultat.Value, båt);
        }

        [Fact]
        public async Task HentEnBåtIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnBåt(It.IsAny<int>())).ReturnsAsync(It.IsAny<Baat>());
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnBåt(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentEnBåtLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnBåt(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnBåt(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Båt ikke funnet i databasen", resultat.Value);
        }

        [Fact]
        public async Task LagreBåtLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBåt(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBåt(It.IsAny<Baater>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task LagreBåtLoggetInnFeilInput()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBåt(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("Båtnavn", "Feil i inputvalidering på server.");

            //Act
            var resultat = await adminController.LagreBåt(It.IsAny<Baater>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalideringen", resultat.Value);
        }

        [Fact]
        public async Task LagreBåtLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBåt(It.IsAny<Baater>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBåt(It.IsAny<Baater>()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
            Assert.Equal("Databasen utilgjengelig. Båt ikke lagret", resultat.Value);
        }


        [Fact]
        public async Task LagreBåtIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBåt(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBåt(It.IsAny<Baater>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettBåtIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.SlettBåt(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettBåt(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettBåtLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.SlettBåt(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettBåt(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task SlettBåtLoggetInnOKMedBillettTest()
        {
            //Arrange
            mockRep.Setup(b => b.SlettBåt(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettBåt(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Båt ikke funnet i databasen eller båt med i eksisterende bestilling(er).", resultat.Value);
        }

        [Fact]
        public async Task EndreBåtIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.EndreBåt(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBåt(It.IsAny<Baater>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task EndreBåtLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.EndreBåt(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBåt(It.IsAny<Baater>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }


        [Fact]
        public async Task EndreBåtLoggetInnOKIkkeOk()
        {
            //Arrange
            mockRep.Setup(b => b.EndreBåt(It.IsAny<Baater>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBåt(It.IsAny<Baater>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Båt ikke funnet", resultat.Value);
        }

        [Fact]
        public async Task EndreBåtLoggetInnFeilInput()
        {
            //Arrange
            mockRep.Setup(b => b.EndreBåt(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("Båtnavn", "Feil i inputvalidering på server.");

            //Act
            var resultat = await adminController.EndreBåt(It.IsAny<Baater>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalideringen", resultat.Value);
        }
        [Fact]
        public async Task HentAlleFerderLoggetInnOK()
        {
            //Arrange
            var ferd1 = new Ferder { FId = 1, BId = It.IsAny<int>(), RId = It.IsAny<int>(), AvreiseTid = It.IsAny<DateTime>().ToString("o"), AnkomstTid = It.IsAny<DateTime>().ToString("o") };
            var ferd2 = new Ferder { FId = 2, BId = It.IsAny<int>(), RId = It.IsAny<int>(), AvreiseTid = It.IsAny<DateTime>().ToString("o"), AnkomstTid = It.IsAny<DateTime>().ToString("o") };

            var ferdListe = new List<Ferder>()
            {
                ferd1,
                ferd2
            };

            mockRep.Setup(b => b.HentAlleFerder()).ReturnsAsync(ferdListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleFerder() as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Ferder>)resultat.Value, ferdListe);
        }

        [Fact]
        public async Task HentAlleFerderIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentAlleFerder()).ReturnsAsync(It.IsAny<List<Ferder>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleFerder() as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentAlleFerderFeilDb()
        {
            mockRep.Setup(b => b.HentAlleFerder()).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleFerder() as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
            Assert.Equal("Databasefeil. Ferder ikke hentet", resultat.Value);
        }

        [Fact]
        public async Task HentEnFerdLoggetInnOk()
        {
            //Arrange
            var ferd = new Ferder
            {
                FId = It.IsAny<int>(),
                RId = It.IsAny<int>(),
                BId = It.IsAny<int>(),
                AvreiseTid = It.IsAny<DateTime>().ToString("o"),
                AnkomstTid = It.IsAny<DateTime>().ToString("o")
            };

            mockRep.Setup(b => b.HentEnFerd(It.IsAny<int>())).ReturnsAsync(ferd);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnFerd(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<Ferder>((Ferder)resultat.Value, ferd);
        }

        [Fact]
        public async Task HentEnFerdIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnFerd(It.IsAny<int>())).ReturnsAsync(It.IsAny<Ferder>());
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnFerd(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentEnFerdLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnFerd(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnFerd(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Ingen ferd funnet", resultat.Value);
        }

        [Fact]
        public async Task LagreFerdLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.LagreFerd(It.IsAny<Ferder>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreFerd(It.IsAny<Ferder>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task LagreFerdLoggetInnFeilInput()
        {
            //Arrange
            mockRep.Setup(b => b.LagreFerd(It.IsAny<Ferder>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("AvreiseTid", "Feil i inputvalidering på server");

            //Act
            var resultat = await adminController.LagreFerd(It.IsAny<Ferder>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering på server", resultat.Value);
        }

        [Fact]
        public async Task LagreFerdLoggetInnIkkeOk()
        {
            //Arrange
            mockRep.Setup(b => b.LagreFerd(It.IsAny<Ferder>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreFerd(It.IsAny<Ferder>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);

            // SKIFT HER!
            //Assert.Equal(" Rute eller båt ikke funnet eller", resultat.Value);
        }


        [Fact]
        public async Task LagreFerdIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.LagreFerd(It.IsAny<Ferder>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreFerd(It.IsAny<Ferder>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettFerdIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.SlettFerd(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettFerd(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettFerdLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.SlettFerd(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettFerd(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }


        // Vet ikke om testmiljøet blir riktig. Sjekk dette om igjen
        [Fact]
        public async Task SlettFerdLoggetInnIkkeOK()
        {
            //Arrange
            mockRep.Setup(b => b.SlettFerd(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettFerd(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Ferd ikke funnet i databasen eller ferd med i eksisterende bestilling(er).", resultat.Value);
        }

        [Fact]
        public async Task EndreFerdIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.EndreFerd(It.IsAny<Ferder>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreFerd(It.IsAny<Ferder>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task EndreFerdLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.EndreFerd(It.IsAny<Ferder>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreFerd(It.IsAny<Ferder>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }


        [Fact]
        public async Task EndreFerdLoggetInnIkkeOK()
        {
            //Arrange
            mockRep.Setup(b => b.EndreFerd(It.IsAny<Ferder>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreFerd(It.IsAny<Ferder>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Ferd, rute eller båt ikke funnet, ferd med i eksisterende bestilling(er) eller databasefeil", resultat.Value);
        }

        [Fact]
        public async Task EndreFerdLoggetInnFeilInput()
        {
            //Arrange
            mockRep.Setup(b => b.EndreFerd(It.IsAny<Ferder>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("Avreisetid", "Feil i inputvalidering på server.");

            //Act
            var resultat = await adminController.EndreFerd(It.IsAny<Ferder>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering på server.", resultat.Value);
        }

        [Fact]
        public async Task HentAlleBilletterLoggetInnOK()
        {
            //Arrange
            var billett1 = new Billetter { Id = 1, FId = It.IsAny<int>(), BId = It.IsAny<int>(), Voksen = true };
            var billett2 = new Billetter { Id = 2, FId = It.IsAny<int>(), BId = It.IsAny<int>(), Voksen = false };

            var billettListe = new List<Billetter>()
            {
                billett1,
                billett2
            };

            mockRep.Setup(b => b.HentAlleBilletter()).ReturnsAsync(billettListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleBilletter() as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Billetter>)resultat.Value, billettListe);
        }

        [Fact]
        public async Task HentAlleBilletterIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentAlleBilletter()).ReturnsAsync(It.IsAny<List<Billetter>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleBilletter() as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentAlleBilletterFeilDb()
        {
            mockRep.Setup(b => b.HentAlleBilletter()).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleBilletter() as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
            Assert.Equal("Databasefeil. Billetter ikke hentet", resultat.Value);
        }

        [Fact]
        public async Task HentBilletterForBestillingLoggetInnOk()
        {
            //Arrange
            var billett1 = new Billetter { Id = 1, FId = It.IsAny<int>(), BId = 1, Voksen = true };
            var billett2 = new Billetter { Id = 2, FId = It.IsAny<int>(), BId = 1, Voksen = false };

            var billettListe = new List<Billetter>()
            {
                billett1,
                billett2
            };
            mockRep.Setup(b => b.HentBilletterForBestilling(1)).ReturnsAsync(billettListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForBestilling(1) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Billetter>)resultat.Value, billettListe);
        }

        [Fact]
        public async Task HentBilletterForBestillingIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentBilletterForBestilling(It.IsAny<int>())).ReturnsAsync(It.IsAny<List<Billetter>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForBestilling(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentBilletterForBestillingIkkeOk()
        {
            mockRep.Setup(b => b.HentBilletterForBestilling(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForBestilling(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Bestilling ikke funnet", resultat.Value);
        }

        [Fact]
        public async Task HentBilletterForFerdLoggetInnOk()
        {
            //Arrange
            var ferd = new Ferd { Id = 1, Baat = It.IsAny<Baat>(), Rute = It.IsAny<Rute>(), AvreiseTid = It.IsAny<DateTime>(), AnkomstTid = It.IsAny<DateTime>() };
            var billett1 = new Billetter { Id = 1, FId = 1, BId = It.IsAny<int>(), Voksen = true };
            var billett2 = new Billetter { Id = 2, FId = 1, BId = It.IsAny<int>(), Voksen = false };

            var billettListe = new List<Billetter>()
            {
                billett1,
                billett2
            };
            mockRep.Setup(b => b.HentBilletterForFerd(1)).ReturnsAsync(billettListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForFerd(1) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Billetter>)resultat.Value, billettListe);
        }

        [Fact]
        public async Task HentBilletterForFerdIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentBilletterForFerd(It.IsAny<int>())).ReturnsAsync(It.IsAny<List<Billetter>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForFerd(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentBilletterForFerdIkkeOk()
        {
            mockRep.Setup(b => b.HentBilletterForFerd(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForFerd(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Ferd ikke funnet", resultat.Value);
        }

        [Fact]
        public async Task HentBilletterForRuteLoggetInnOk()
        {
            //Arrange
            var rute = new Rute { Id = 1, Startpunkt = It.IsAny<string>(), Endepunkt = It.IsAny<string>(), Pris = It.IsAny<double>() };
            var ferd = new Ferd { Id = 1, Baat = It.IsAny<Baat>(), Rute = rute, AvreiseTid = It.IsAny<DateTime>(), AnkomstTid = It.IsAny<DateTime>() };
            var billett1 = new Billetter { Id = 1, FId = 1, BId = It.IsAny<int>(), Voksen = true };
            var billett2 = new Billetter { Id = 2, FId = 1, BId = It.IsAny<int>(), Voksen = false };

            var billettListe = new List<Billetter>()
            {
                billett1,
                billett2
            };
            mockRep.Setup(b => b.HentBilletterForRute(1)).ReturnsAsync(billettListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForRute(1) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Billetter>)resultat.Value, billettListe);
        }

        [Fact]
        public async Task HentBilletterForRuteIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentBilletterForRute(It.IsAny<int>())).ReturnsAsync(It.IsAny<List<Billetter>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForRute(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentBilletterForRuteIkkeOk()
        {
            mockRep.Setup(b => b.HentBilletterForRute(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForRute(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Rute ikke funnet", resultat.Value);
        }

        [Fact]
        public async Task HentBilletterForBåtLoggetInnOk()
        {
            //Arrange
            var båt = new Baat { Id = 1, Navn = It.IsAny<string>() };
            var ferd = new Ferd { Id = 1, Baat = båt, Rute = It.IsAny<Rute>(), AvreiseTid = It.IsAny<DateTime>(), AnkomstTid = It.IsAny<DateTime>() };
            var billett1 = new Billetter { Id = 1, FId = 1, BId = It.IsAny<int>(), Voksen = true };
            var billett2 = new Billetter { Id = 2, FId = 1, BId = It.IsAny<int>(), Voksen = false };

            var billettListe = new List<Billetter>()
            {
                billett1,
                billett2
            };
            mockRep.Setup(b => b.HentBilletterForBåt(1)).ReturnsAsync(billettListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForBåt(1) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Billetter>)resultat.Value, billettListe);
        }

        [Fact]
        public async Task HentBestillingerForKundeIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentBestillingerForKunde(It.IsAny<int>())).ReturnsAsync(It.IsAny<List<Bestilling>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBestillingerForKunde(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentBestillingerForKundeIkkeOk()
        {
            mockRep.Setup(b => b.HentBestillingerForKunde(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBestillingerForKunde(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Kunde ikke funnet", resultat.Value);
        }

        [Fact]
        public async Task HentBestillingerForKundeLoggetInnOk()
        {
            //Arrange
            var bestilling1 = new Bestilling { Id = 1, KId = 1, Betalt = true };
            var bestilling2 = new Bestilling { Id = 1, KId = 1, Betalt = true };
            List<Bestilling> bestillingListe = new List<Bestilling>();
            bestillingListe.Add(bestilling1);
            bestillingListe.Add(bestilling2);

            mockRep.Setup(b => b.HentBestillingerForKunde(1)).ReturnsAsync(bestillingListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBestillingerForKunde(1) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Bestilling>)resultat.Value, bestillingListe);
        }

        [Fact]
        public async Task HentBilletterForBåtIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentBilletterForBåt(It.IsAny<int>())).ReturnsAsync(It.IsAny<List<Billetter>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForBåt(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentBilletterForBåtIkkeOk()
        {
            mockRep.Setup(b => b.HentBilletterForBåt(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentBilletterForBåt(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Båt ikke funnet.", resultat.Value);
        }

        [Fact]
        public async Task HentFerdRuterForBestillingLoggetInnOk()
        {
            //Arrange
            FerdRute ferdRute1 = new FerdRute { FId = It.IsAny<int>(), AnkomstTid = It.IsAny<string>(), AvreiseTid = It.IsAny<string>(), Strekning = "Oslo - Bergen" };
            FerdRute ferdRute2 = new FerdRute { FId = It.IsAny<int>(), AnkomstTid = It.IsAny<string>(), AvreiseTid = It.IsAny<string>(), Strekning = "Oslo - Bergen" };

            List<FerdRute> ferdRuteListe = new List<FerdRute>() { ferdRute1, ferdRute2 };
            

            mockRep.Setup(b => b.HentFerdRuterForBestilling(It.IsAny<int>())).ReturnsAsync(ferdRuteListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentFerdRuterForBestilling(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<FerdRute>)resultat.Value, ferdRuteListe);
        }

        [Fact]
        public async Task HentFerdRuterForBestillingIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentFerdRuterForBestilling(It.IsAny<int>())).ReturnsAsync(It.IsAny<List<FerdRute>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentFerdRuterForBestilling(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentFerdRuterForBestillingIkkeOk()
        {
            mockRep.Setup(b => b.HentFerdRuterForBestilling(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentFerdRuterForBestilling(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Bestilling ikke funnet.", resultat.Value);
        }

        [Fact]
        public async Task HentEnBillettLoggetInnOk()
        {
            //Arrange
            var billett = new Billetter
            {
                Id = It.IsAny<int>(),
                BId = It.IsAny<int>(),
                FId = It.IsAny<int>(),
                Voksen = true
            };

            mockRep.Setup(b => b.HentEnBillett(It.IsAny<int>())).ReturnsAsync(billett);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnBillett(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<Billetter>((Billetter)resultat.Value, billett);
        }

        [Fact]
        public async Task HentEnBillettIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnBillett(It.IsAny<int>())).ReturnsAsync(It.IsAny<Billetter>());
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnBillett(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentEnBillettLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnBillett(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnBillett(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Ingen billett funnet", resultat.Value);
        }

        [Fact]
        public async Task LagreBillettLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBillett(It.IsAny<Billetter>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBillett(It.IsAny<Billetter>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task LagreBillettLoggetInnIkkeOk()
        {
            //Arrange
            Bestilling bestilling = new Bestilling { Id = 1, KId = It.IsAny<int>(), Betalt = true, Totalpris = It.IsAny<double>() };
            Billetter billett = new Billetter { Id = It.IsAny<int>(), BId = 1, FId = It.IsAny<int>(), Voksen = false };
            mockRep.Setup(b => b.HentEnBestilling(1)).ReturnsAsync(bestilling);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBillett(billett) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Bestilling eller ferd ikke funnet, ferden har vært eller bestillingen er betalt", resultat.Value);
        }


        [Fact]
        public async Task LagreBillettIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBillett(It.IsAny<Billetter>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBillett(It.IsAny<Billetter>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettBillettIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.SlettBillett(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettBillett(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettBillettLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.SlettBillett(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettBillett(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        // Kan sikkert også implementeres fancy hvis vi får tid
        [Fact]
        public async Task SlettBillettLoggetInnIkkeOK()
        {

            //Arrange
            mockRep.Setup(b => b.SlettBillett(It.IsAny<int>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettBillett(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Billett ikke funnet eller inneholder gjennomført, ubetalt reise eller ugjennomført, betalt reise", resultat.Value);
        }

        [Fact]
        public async Task EndreBillettIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.EndreBillett(It.IsAny<Billetter>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBillett(It.IsAny<int>(),It.IsAny<Billetter>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task EndreBillettLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.EndreBillett(It.IsAny<Billetter>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBillett(It.IsAny<int>(),It.IsAny<Billetter>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task EndreBillettLoggetInnIkkeOK()
        {
            //Arrange

            mockRep.Setup(b => b.EndreBillett(It.IsAny<Billetter>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBillett(It.IsAny<int>(),It.IsAny<Billetter>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Billett ikke funnet, eller billetten allerede brukt eller billetten er betalt", resultat.Value);
        }

        [Fact]
        public async Task HentAlleBestillingerLoggetInnOK()
        {
            //Arrange
            var bestilling1 = new Bestilling { Id = 1, KId = It.IsAny<int>(), Betalt = true, Totalpris = It.IsAny<double>() };
            var bestilling2 = new Bestilling { Id = 2, KId = It.IsAny<int>(), Betalt = false, Totalpris = It.IsAny<double>() };

            var bestillingsListe = new List<Bestilling>()
            {
                bestilling1,
                bestilling2
            };

            mockRep.Setup(b => b.HentAlleBestillinger()).ReturnsAsync(bestillingsListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleBestillinger() as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Bestilling>)resultat.Value, bestillingsListe);
        }

        [Fact]
        public async Task HentAlleBestillingerIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentAlleBestillinger()).ReturnsAsync(It.IsAny<List<Bestilling>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleBestillinger() as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentAlleBestillingerFeilDb()
        {
            mockRep.Setup(b => b.HentAlleBilletter()).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleBestillinger() as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
            Assert.Equal("Databasefeil. Bestillinger ikke hentet", resultat.Value);
        }

        [Fact]
        public async Task HentEnBestillingLoggetInnOk()
        {
            //Arrange
            var bestilling = new Bestilling
            {
                Id = It.IsAny<int>(),
                KId = It.IsAny<int>(),
                Betalt = true,
                Totalpris = It.IsAny<int>()
            };

            mockRep.Setup(b => b.HentEnBestilling(It.IsAny<int>())).ReturnsAsync(bestilling);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnBestilling(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<Bestilling>((Bestilling)resultat.Value, bestilling);
        }

        [Fact]
        public async Task HentEnBestillingIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnBestilling(It.IsAny<int>())).ReturnsAsync(It.IsAny<Bestilling>());
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnBestilling(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentEnBestillingLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnBestilling(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnBestilling(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Ingen bestilling funnet", resultat.Value);
        }

        [Fact]
        public async Task SlettBestillingIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.SlettBestilling(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettBestilling(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettBestillingLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.SlettBestilling(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettBestilling(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task SlettBestillingLoggetInnIkkeOK()
        {
            //Arrange
            mockRep.Setup(b => b.SlettBillett(It.IsAny<int>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettBillett(It.IsAny<int>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Billett ikke funnet eller inneholder gjennomført, ubetalt reise eller ugjennomført, betalt reise", resultat.Value);
        }

        [Fact]
        public async Task EndreBestillingIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.EndreBestilling(It.IsAny<Bestilling>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBestilling(It.IsAny<Bestilling>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task EndreBestillingLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.EndreBestilling(It.IsAny<Bestilling>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBestilling(It.IsAny<Bestilling>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }


        [Fact]
        public async Task EndreBestillingLoggetInnIkkeOK()
        {
            //Arrange

            mockRep.Setup(b => b.EndreBestilling(It.IsAny<Bestilling>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBestilling(It.IsAny<Bestilling>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Bestilling eller kunde ikke funnet, eller bestillingen er betalt eller bestillingen inneholder gjennomførte ubetalte reiser", resultat.Value);
        }
        [Fact]
        public async Task EndreKundeIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.EndrePerson(It.IsAny<Personer>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreKunde(It.IsAny<int>(), It.IsAny<Personer>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task EndreKundeLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.EndrePerson(It.IsAny<Personer>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreKunde(It.IsAny<int>(), It.IsAny<Personer>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

     
        [Fact]
        public async Task EndreKundeLoggetInnIkkeOK()
        {
            //Arrange
            mockRep.Setup(b => b.EndrePerson(It.IsAny<Personer>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreKunde(It.IsAny<int>(), It.IsAny<Personer>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Kunde ikke funnet i databasen", resultat.Value);
        }

        [Fact]
        public async Task EndreKundeLoggetInnFeilModel()
        {
            //Arrange
            mockRep.Setup(b => b.EndrePerson(It.IsAny<Personer>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("Fornavn", "Feil i inputvalideringen");

            //Act
            var resultat = await adminController.EndreKunde(It.IsAny<int>(), It.IsAny<Personer>()) as BadRequestObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalideringen", resultat.Value);
        }

        [Fact]
        public async Task SlettKundeIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.SlettKunde(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettKunde(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettKundeLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.SlettKunde(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettKunde(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task SlettKundeLoggetInnIkkeOK()
        {
            //Arrange
            mockRep.Setup(b => b.SlettKunde(1)).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettKunde(1) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Kunde ikke funnet eller kunden har bestillinger i database", resultat.Value);
        }

        [Fact]
        public async Task HentAlleKunderLoggetInnOK()
        {
            //Arrange
            var kunde1 = new Personer { Id = 1, Fornavn = "Kunde", Etternavn = "Kundesen", Epost = "kunde@kunde.no", Telefonnr = "11111111" };
            var kunde2 = new Personer { Id = 2, Fornavn = "Viggo", Etternavn = "Torgersen", Epost = "kunde2@kunde.no", Telefonnr = "44444444" };

            var kundeListe = new List<Personer>();
            kundeListe.Add(kunde1);
            kundeListe.Add(kunde2);

            mockRep.Setup(b => b.HentAlleKunder()).ReturnsAsync(kundeListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleKunder() as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Personer>)resultat.Value, kundeListe);
        }

        [Fact]
        public async Task HentAlleKunderIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentAlleKunder()).ReturnsAsync(It.IsAny<List<Personer>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleKunder() as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentAlleKunderFeilDb()
        {
            mockRep.Setup(b => b.HentAlleKunder()).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleKunder() as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
            Assert.Equal("Databasefeil. Ingen kunder hentet", resultat.Value);
        }

        [Fact]
        public async Task HentAlleAnsatteLoggetInnOK()
        {
            //Arrange
            var ansatt = new Personer { Id = 1, Fornavn = "Kunde", Etternavn = "Kundesen", Epost = "kunde@kunde.no", Telefonnr = "11111111" };
            var ansatt2 = new Personer { Id = 2, Fornavn = "Tor", Etternavn = "Latterbrøl", Epost = "kunde2@kunde.no", Telefonnr = "44444444" };

            var ansattListe = new List<Personer>();
            ansattListe.Add(ansatt);
            ansattListe.Add(ansatt2);

            mockRep.Setup(b => b.HentAlleAnsatte()).ReturnsAsync(ansattListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleAnsatte() as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Personer>)resultat.Value, ansattListe);
        }

        [Fact]
        public async Task HentAlleAnsatteIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentAlleAnsatte()).ReturnsAsync(It.IsAny<List<Personer>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleAnsatte() as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentAlleAnsatteFeilDb()
        {
            mockRep.Setup(b => b.HentAlleAnsatte()).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleAnsatte() as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
            Assert.Equal("Databasefeil. Ingen ansatte hentet", resultat.Value);
        }

        [Fact]
        public async Task HentEnKundeLoggetInnOk()
        {
            //Arrange
            var kunde = new Personer
            {
                Id = 1,
                Fornavn = "Kunde",
                Etternavn = "Kundesen",
                Epost = "kunde@kundesen.no",
                Telefonnr = "22222222"
            };

            mockRep.Setup(b => b.HentEnKunde(It.IsAny<int>())).ReturnsAsync(kunde);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnKunde(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<Personer>((Personer)resultat.Value, kunde);
        }

        [Fact]
        public async Task HentEnKundeIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnKunde(It.IsAny<int>())).ReturnsAsync(It.IsAny<Personer>());
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnKunde(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentEnKundeLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnKunde(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnKunde(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Kunde ikke funnet", resultat.Value);
        }

        [Fact]
        public async Task LoggInnOK()
        {
            //Arrange
            mockRep.Setup(b => b.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(It.Is<int>(i => i != 0));
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LoggInn(It.IsAny<Bruker>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
        }

        [Fact]
        public async Task HentEnAnsattLoggetInnOk()
        {
            //Arrange
            var ansatt = new Personer
            {
                Id = 1,
                Fornavn = "Ansatt",
                Etternavn = "Jobbemann",
                Epost = "ansatt@ansatte.no",
                Telefonnr = "22224444"
            };

            mockRep.Setup(b => b.HentEnAnsatt(It.IsAny<int>())).ReturnsAsync(ansatt);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnAnsatt(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<Personer>((Personer)resultat.Value, ansatt);
        }

        [Fact]
        public async Task HentEnAnsattIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnAnsatt(It.IsAny<int>())).ReturnsAsync(It.IsAny<Personer>());
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnAnsatt(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentEnAnsattLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnAnsatt(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnAnsatt(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Ansatt ikke funnet", resultat.Value);
        }

        [Fact]
        public async Task LagreBestillingLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBestilling(It.IsAny<Bestilling>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBestilling(It.IsAny<Bestilling>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task LagreBestillingLoggetInnIkkeOk()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBestilling(It.IsAny<Bestilling>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBestilling(It.IsAny<Bestilling>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Kunde eller ferd ikke funnet", resultat.Value);
        }


        [Fact]
        public async Task LagreBestillingIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.LagreBestilling(It.IsAny<Bestilling>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreBestilling(It.IsAny<Bestilling>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task LagreKundeLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.LagreKunde(It.IsAny<Personer>())).ReturnsAsync(It.Is<int>(i => i != 0));
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreKunde(It.IsAny<Personer>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task LagreKundeLoggetInnFeilInput()
        {
            //Arrange
            mockRep.Setup(b => b.LagreKunde(It.IsAny<Personer>())).ReturnsAsync(It.Is<int>(i => i != 0));
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("Fornavn", "Feil i inputvalidering på server.");

            //Act
            var resultat = await adminController.LagreKunde(It.IsAny<Personer>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering på server", resultat.Value);
        }

        [Fact]
        public async Task LagreKundeLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.LagreKunde(It.IsAny<Personer>())).ReturnsAsync(It.Is<int>(i => i != 0));
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreKunde(It.IsAny<Personer>()) as ObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
            Assert.Equal("Databasefeil. Kunden ikke opprettet", resultat.Value);
        }


        [Fact]
        public async Task LagreKundeIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.LagreKunde(It.IsAny<Personer>())).ReturnsAsync(It.Is<int>(i => i != 0));
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreKunde(It.IsAny<Personer>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task EndreBrukerIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.EndreBruker(It.IsAny<Bruker>(), It.IsAny<string>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBruker(It.IsAny<Bruker>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task EndreBrukerLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.EndreBruker(It.IsAny<Bruker>(), It.IsAny<string>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBruker(It.IsAny<Bruker>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }


        [Fact]
        public async Task EndreBrukerLoggetInnIkkeOK()
        {
            //Arrange

            mockRep.Setup(b => b.EndreBruker(It.IsAny<Bruker>(), It.IsAny<string>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreBruker(It.IsAny<Bruker>()) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("Bestilling eller kunde ikke funnet, eller bestillingen er betalt eller bestillingen inneholder gjennomførte ubetalte reiser", resultat.Value);
        }

        [Fact]
        public void HentBrukerLoggetInnOK()
        {
            //Arrange
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockSession[_brukernavn] = _brukernavn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = adminController.HentBruker() as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal("brukernavn",(string)resultat.Value);
        }

        [Fact]
        public void HentBrukerIkkeLoggetInn()
        {
            //Arrange
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockSession[_brukernavn] = _brukernavn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = adminController.HentBruker() as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", (string)resultat.Value);
        }

        [Fact]
        public void HentBrukerFeilDb()
        {   
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = adminController.HentBruker() as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.False((bool)resultat.Value);
        }

        //[Fact]
        //public async Task HentProfilOk()
        //{
        //    //Arrange
            
        //    //Act
            
        //    //Assert
        //    }

        //[Fact]
        //public async Task HentProfilIkkeOk()
        //{
            
        //    //Act
            
        //    //Assert
        //    }

        //[Fact]
        //public async Task HentProfilIkkeLoggetInn()
        //{
         
        //   //Arrange
        //    var adminController = new AdminController(mockRep.Object, mockLog.Object);
        //    //Act
        //    var resultat = await adminController.HentProfil() as ObjectResult;

        //    //Assert
        //    Assert.Equal((int)HttpStatusCode.InternalServerError, resultat.StatusCode);
        //    Assert.Equal("Databasefeil. Bestillinger ikke hentet", resultat.Value);
        //}


    }
}

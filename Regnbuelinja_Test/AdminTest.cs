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

        private readonly Mock<IBestillingRepository> mockRep = new Mock<IBestillingRepository>();
        private readonly Mock<ILogger<AdminController>> mockLog = new Mock<ILogger<AdminController>>();
        private readonly Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
        private readonly MockHttpSession mockSession = new MockHttpSession();

        [Fact]
        public async Task LoggInnOK()
        {
            //Arrange
            mockRep.Setup(b => b.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LoggInn(It.IsAny<Bruker>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task LoggInnFeilBrukerinput()
        {
            //Arrange
            mockRep.Setup(b => b.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(true);

            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("Brukernavn", "Feil i inputvalidering p� server");

            //Act
            var resultat = await adminController.LoggInn(It.IsAny<Bruker>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalideringen p� server", resultat.Value);
        }

        [Fact]
        public async Task LoggInnFeilBrukernavnEllerPassord()
        {
            //Arrange
            mockRep.Setup(b => b.LoggInn(It.IsAny<Bruker>())).ReturnsAsync(false);

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
        public async Task LoggUt()
        {
            
            //Arrange
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            adminController.LoggUt();

            //Assert
            Assert.Equal(_ikkeLoggetInn, mockSession[_loggetInn]);
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

            adminController.ModelState.AddModelError("Brukernavn", "Feil i inputvalidering p� server.");

            //Act
            var resultat = await adminController.LagreBruker(It.IsAny<Bruker>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering p� server", resultat.Value);
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
            var resultat = await adminController.LagreBruker(It.IsAny<Bruker>()) as ServiceUnavailableResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.ServiceUnavailable, resultat.StatusCode);
            Assert.Equal("Databasefeil. Bruker ikke opprettet", resultat.ViewName);
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
            var rute1 = new Rute { Id = 1, Startpunkt = "Haiti", Endepunkt = "Oslo", Pris = 9999.99};
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
            mockRep.Setup(b => b.HentAlleRuter()).ReturnsAsync(()=>null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleRuter() as ServiceUnavailableResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.ServiceUnavailable, resultat.StatusCode);
            Assert.Equal("Databasefeil. Ruter ikke hentet", resultat.ViewName);
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
            mockRep.Setup(b => b.HentEnRute(It.IsAny<int>())).ReturnsAsync(()=>null);
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

            adminController.ModelState.AddModelError("Avreisehavn", "Feil i inputvalidering p� server.");

            //Act
            var resultat = await adminController.LagreRute(It.IsAny<Ruter>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering p� server.", resultat.Value);
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
            var resultat = await adminController.LagreRute(It.IsAny<Ruter>()) as ServiceUnavailableResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.ServiceUnavailable, resultat.StatusCode);
            Assert.Equal("Databasefeil. Rute ikke lagret", resultat.ViewName);
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


        // Vet ikke om testmilj�et blir riktig. Sjekk dette om igjen
        [Fact]
        public async Task SlettRuteLoggetInnOKMedBillettTest()
        {
            //Arrange
            var ruter = new Ruter { Avreisehavn = "Haiti", Ankomsthavn = "Oslo", Pris = 9999.99 };
            var ferder = new Ferder { AnkomstTid = It.IsAny<DateTime>().ToString("o"), AvreiseTid = It.IsAny<DateTime>().ToString("o"), RId = 1, BId = It.IsAny<int>() };
            var billetter = new Billetter { BId = It.IsAny<int>(), FId = 1, Voksen = true };

            mockRep.Setup(b => b.LagreRute(ruter)).ReturnsAsync(true);
            mockRep.Setup(b => b.LagreFerd(ferder)).ReturnsAsync(true);
            mockRep.Setup(b => b.LagreBillett(billetter)).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettRute(1) as NotFoundObjectResult;

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


        // Vet ikke om testmilj�et blir riktig. Sjekk dette om igjen
        [Fact]
        public async Task EndreRuteLoggetInnOKMedBillettTest()
        {
            //Arrange
            var ruter = new Ruter { Avreisehavn = "Haiti", Ankomsthavn = "Oslo", Pris = 9999.99 };
            var ruter2 = new Ruter {Id = 1, Avreisehavn = "New York", Ankomsthavn = "Oslo", Pris = 250.00 };
            var ferder = new Ferder { AnkomstTid = It.IsAny<DateTime>().ToString("o"), AvreiseTid = It.IsAny<DateTime>().ToString("o"), RId = 1, BId = It.IsAny<int>() };
            var billetter = new Billetter { BId = It.IsAny<int>(), FId = 1, Voksen = true };

            mockRep.Setup(b => b.LagreRute(ruter)).ReturnsAsync(true);
            mockRep.Setup(b => b.LagreFerd(ferder)).ReturnsAsync(true);
            mockRep.Setup(b => b.LagreBillett(billetter)).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreRute(ruter2) as NotFoundObjectResult;

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

            adminController.ModelState.AddModelError("Avreisehavn", "Feil i inputvalidering p� server.");

            //Act
            var resultat = await adminController.EndreRute(It.IsAny<Ruter>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalidering p� server.", resultat.Value);
        }
        [Fact]
        public async Task HentAlleB�terLoggetInnOK()
        {
            //Arrange
            var b�t1 = new Baat { Id = 1, Navn = "Kontiki"};
            var b�t2 = new Baat { Id = 2, Navn = "HM Queen Victoria"};

            var b�tListe = new List<Baat>();
            b�tListe.Add(b�t1);
            b�tListe.Add(b�t2);

            mockRep.Setup(b => b.HentAlleB�ter()).ReturnsAsync(b�tListe);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleB�ter() as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal((List<Baat>)resultat.Value, b�tListe);
        }

        [Fact]
        public async Task HentAlleB�terIkkeLoggetInn()
        {
            mockRep.Setup(b => b.HentAlleB�ter()).ReturnsAsync(It.IsAny<List<Baat>>);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleB�ter() as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentAlleB�terFeilDb()
        {
            mockRep.Setup(b => b.HentAlleB�ter()).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentAlleB�ter() as ServiceUnavailableResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.ServiceUnavailable, resultat.StatusCode);
            Assert.Equal("Databasefeil. B�ter ikke hentet", resultat.ViewName);
        }

        [Fact]
        public async Task HentEnB�tLoggetInnOk()
        {
            //Arrange
            var b�t = new Baat
            {
                Id = 1,
                Navn = "Kontiki"
            };

            mockRep.Setup(b => b.HentEnB�t(It.IsAny<int>())).ReturnsAsync(b�t);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnB�t(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal<Baat>((Baat)resultat.Value, b�t);
        }

        [Fact]
        public async Task HentEnB�tIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnB�t(It.IsAny<int>())).ReturnsAsync(It.IsAny<Baat>());
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnB�t(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task HentEnB�tLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.HentEnB�t(It.IsAny<int>())).ReturnsAsync(() => null);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.HentEnB�t(It.IsAny<int>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("B�t ikke funnet i databasen", resultat.Value);
        }

        [Fact]
        public async Task LagreB�tLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.LagreB�t(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreB�t(It.IsAny<Baater>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }

        [Fact]
        public async Task LagreB�tLoggetInnFeilInput()
        {
            //Arrange
            mockRep.Setup(b => b.LagreB�t(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("B�tnavn", "Feil i inputvalidering p� server.");

            //Act
            var resultat = await adminController.LagreB�t(It.IsAny<Baater>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalideringen", resultat.Value);
        }

        [Fact]
        public async Task LagreB�tLoggetInnFeilDb()
        {
            //Arrange
            mockRep.Setup(b => b.LagreB�t(It.IsAny<Baater>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreB�t(It.IsAny<Baater>()) as ServiceUnavailableResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.ServiceUnavailable, resultat.StatusCode);
            Assert.Equal("Databasen utilgjengelig. B�t ikke lagret", resultat.ViewName);
        }


        [Fact]
        public async Task LagreB�tIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.LagreB�t(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.LagreB�t(It.IsAny<Baater>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettB�tIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.SlettB�t(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettB�t(It.IsAny<int>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task SlettB�tLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.SlettB�t(It.IsAny<int>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettB�t(It.IsAny<int>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }


        // Vet ikke om testmilj�et blir riktig. Sjekk dette om igjen
        [Fact]
        public async Task SlettB�tLoggetInnOKMedBillettTest()
        {
            //Arrange
            var b�t = new Baater { B�tnavn = "Kontiki" };
            var ferder = new Ferder { AnkomstTid = It.IsAny<DateTime>().ToString("o"), AvreiseTid = It.IsAny<DateTime>().ToString("o"), RId = It.IsAny<int>(), BId = 1 };
            var billetter = new Billetter { BId = It.IsAny<int>(), FId = 1, Voksen = true };

            mockRep.Setup(b => b.LagreB�t(b�t)).ReturnsAsync(true);
            mockRep.Setup(b => b.LagreFerd(ferder)).ReturnsAsync(true);
            mockRep.Setup(b => b.LagreBillett(billetter)).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.SlettB�t(1) as NotFoundObjectResult;

            // Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("B�t ikke funnet i databasen eller b�t med i eksisterende bestilling(er).", resultat.Value);
        }

        [Fact]
        public async Task EndreB�tIkkeLoggetInn()
        {
            //Arrange
            mockRep.Setup(b => b.EndreB�t(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _ikkeLoggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreB�t(It.IsAny<Baater>()) as UnauthorizedObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.Unauthorized, resultat.StatusCode);
            Assert.Equal("Ikke logget inn", resultat.Value);
        }

        [Fact]
        public async Task EndreB�tLoggetInnOk()
        {
            //Arrange
            mockRep.Setup(b => b.EndreB�t(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreB�t(It.IsAny<Baater>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.True((bool)resultat.Value);
        }


        // Vet ikke om testmilj�et blir riktig. Sjekk dette om igjen
        [Fact]
        public async Task EndreB�tLoggetInnOKIkkeOk()
        {
            //Arrange
            mockRep.Setup(b => b.EndreB�t(It.IsAny<Baater>())).ReturnsAsync(false);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            //Act
            var resultat = await adminController.EndreB�t(It.IsAny<Baater>()) as NotFoundObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.NotFound, resultat.StatusCode);
            Assert.Equal("B�t ikke funnet", resultat.Value);
        }

        [Fact]
        public async Task EndreB�tLoggetInnFeilInput()
        {
            //Arrange
            mockRep.Setup(b => b.EndreB�t(It.IsAny<Baater>())).ReturnsAsync(true);
            var adminController = new AdminController(mockRep.Object, mockLog.Object);

            mockSession[_loggetInn] = _loggetInn;
            mockHttpContext.Setup(s => s.Session).Returns(mockSession);
            adminController.ControllerContext.HttpContext = mockHttpContext.Object;

            adminController.ModelState.AddModelError("B�tnavn", "Feil i inputvalidering p� server.");

            //Act
            var resultat = await adminController.EndreB�t(It.IsAny<Baater>()) as BadRequestObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.BadRequest, resultat.StatusCode);
            Assert.Equal("Feil i inputvalideringen", resultat.Value);
        }
    }
}

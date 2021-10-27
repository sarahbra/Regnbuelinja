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

        //[Fact]
        //public async Task SlettRuteLoggetInnOKMedFerdTest()
        //{
        //    //Arrange
        //    var baat = new Baat { BId = 1, Navn = "Kharon" };
        //    var rute = new Rute { RId = 1, Startpunkt = "Haiti", Endepunkt = "Oslo", Pris = 9999.99 };
        //    var ferd = new Ferd { FId = 1, AnkomstTid = It.IsAny<DateTime>(), AvreiseTid = It.IsAny<DateTime>(), Baat = baat, Rute = rute };

        //    mockRep.Setup(b => b.LagreFerd(ferd)).ReturnsAsync(true);
        //    mockRep.Setup(b => b.SlettRute(rute.RId)).ReturnsAsync(true);
        //    var adminController = new AdminController(mockRep.Object, mockLog.Object);

        //    mockSession[_loggetInn] = _loggetInn;
        //    mockHttpContext.Setup(s => s.Session).Returns(mockSession);
        //    adminController.ControllerContext.HttpContext = mockHttpContext.Object;

        //    //Act
        //    var resultat = await adminController.SlettRute(rute.RId) as OkObjectResult;
        //    var resultat2 = await adminController.HentEnFerd(ferd.FId) as NotFoundObjectResult;

        //    //Assert
        //    Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
        //    Assert.True((bool)resultat.Value);

        //    Assert.Equal((int)HttpStatusCode.NotFound, resultat2.StatusCode);
        //    Assert.Equal("Ingen ferd funnet", resultat2.Value);
        //}

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
            var resultat = await adminController.HentAlleRuter() as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal("Feil i databasen. Ingen ruter hentet.", resultat.Value);
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
            Assert.Equal("Vellykket! Rute lagret i databasen", resultat.Value);
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

            adminController.ModelState.AddModelError("Startpunkt", "Feil i inputvalidering p� server.");

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
            var resultat = await adminController.LagreRute(It.IsAny<Ruter>()) as OkObjectResult;

            //Assert
            Assert.Equal((int)HttpStatusCode.OK, resultat.StatusCode);
            Assert.Equal("Feil i databasen. Pr�v p� nytt", resultat.Value);
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
    }
}

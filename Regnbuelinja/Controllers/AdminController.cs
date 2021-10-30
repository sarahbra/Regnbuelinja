using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Regnbuelinja.DAL;
using Regnbuelinja.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Regnbuelinja.Controllers
{
    [ApiController]
    [Route("/api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IBestillingRepository _db;

        private readonly ILogger<AdminController> _log;
        private const string _loggetInn = "loggetInn";

        public AdminController(IBestillingRepository db, ILogger<AdminController> log)
        {
            _db = db;
            _log = log;
        }

        [HttpPost("ruter")]
        public async Task<ActionResult> LagreRute(Ruter rute)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            if (ModelState.IsValid)
            {
                bool lagret = await _db.LagreRute(rute);
                if(lagret)
                {
                    _log.LogInformation("AdminController.cs: LagreRute: Vellykket! Rute lagret");
                    return Ok(lagret);
                }
                _log.LogInformation("AdminController.cs: LagreRute: Databasefeil. Rute ikke lagret");
                return new ServiceUnavailableResult("Databasefeil. Rute ikke lagret");
            }
            _log.LogInformation("AdminController.cs: LagreRute: Feil i inputvalideringen.");
            return BadRequest("Feil i inputvalidering på server.");
        }

        [HttpPut("rute/{id}")]
        public async Task<ActionResult> EndreRute(Ruter rute)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            if(ModelState.IsValid)
            {
                bool RuteEndret = await _db.EndreRute(rute);
                if (RuteEndret)
                {
                    _log.LogInformation("AdminController.cs: EndreRute: Vellykket! Rute endret");
                    return Ok("Vellykket. Rute endret");
                }
                _log.LogInformation("AdminController.cs: EndreRute: Databasefeil. Rute ikke endret");
                return NotFound("Rute ikke funnet eller rute med i eksisterende bestilling(er).");
            }
            _log.LogInformation("AdminController.cs: EndreRute: Feil i inputvalideringen.");
            return BadRequest("Feil i inputvalidering på server.");
        }

        [HttpGet("ruter")]
        public async Task<ActionResult> HentAlleRuter()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            List<Rute> ruter = await _db.HentAlleRuter();
            if(ruter != null)
            {
                _log.LogInformation("AdminController.cs: HentAlleRuter: Vellykket. Ruter hentet");
                return Ok(ruter);
            }
            _log.LogInformation("AdminController.cs: HentAlleRuter: Feil i databasen eller ingen ruter lagret");
            return new ServiceUnavailableResult("Databasefeil. Ruter ikke hentet");
        }

        [HttpGet("rute/{id}")]
        public async Task<ActionResult> HentEnRute(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            Rute hentetRute = await _db.HentEnRute(id);
            if (hentetRute != null)
            {
                _log.LogInformation("AdminController.cs: HentEnRute: Vellykket. Rute hentet");
                return Ok(hentetRute);
            }
            _log.LogInformation("AdminController.cs: HentEnRute: Ingen rute funnet i databasen");
            return NotFound("Ingen rute funnet");
        }

        [HttpDelete("rute/{id}")]
        public async Task<ActionResult> SlettRute(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            bool slettet = await _db.SlettRute(id);
            if(slettet)
            {
                _log.LogInformation("AdminController.cs: SlettRute: Rute slettet.");
                return Ok(slettet);
            }
            _log.LogInformation("AdminController.cs: SlettRute: Rute finnes ikke eller databasefeil");
            return NotFound("Rute ikke funnet eller rute med i eksisterende bestilling(er).");

        }

        [HttpPost("baater")]
        public async Task<ActionResult> LagreBåt(Baater båt)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            if (ModelState.IsValid)
            {
                bool lagretBåt = await _db.LagreBåt(båt);
                if(lagretBåt)
                {
                    _log.LogInformation("AdminController.cs: LagreBåt: Båt lagret i databasen");
                    return Ok(lagretBåt);
                }
                _log.LogInformation("AdminController.cs: LagreBåt: Databasefeil. Båt kunne ikke lagres");
                return new ServiceUnavailableResult("Databasen utilgjengelig. Båt ikke lagret");
            }
            _log.LogInformation("AdminController.cs: LagreBåt: Feil i inputvalideringen. Båt ikke lagret");
            return BadRequest("Feil i inputvalideringen");
        }

        [HttpPut("baat/{id}")]
        public async Task<ActionResult> EndreBåt(Baater båt)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            if (ModelState.IsValid)
            {
                bool endretBåt = await _db.EndreBåt(båt);
                if (endretBåt)
                {
                    _log.LogInformation("AdminController.cs: EndreBåt: Båt endret i databasen");
                    return Ok("Vellykket! Båt endret i databasen");
                }
                _log.LogInformation("AdminController.cs: EndreBåt: Databasefeil. Båt kunne ikke endres");
                return NotFound("Båt ikke funnet i databasen eller databasefeil.");
            }
            _log.LogInformation("AdminController.cs: EndreBåt: Feil i inputvalideringen. Båt ikke endret");
            return BadRequest("Feil i inputvalideringen");
        }

        [HttpDelete("baat/{id}")]
        public async Task<ActionResult> SlettBåt(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            bool slettetBåt = await _db.SlettBåt(id);
            if (slettetBåt)
            {
                _log.LogInformation("AdminController.cs: SlettBåt: Båt slettet fra databasen");
                return Ok("Vellykket! Båt slettet");
            }
            _log.LogInformation("AdminController.cs: SlettBåt: Databasefeil. Båt kunne ikke endres");
            return NotFound("Båt ikke funnet i databasen eller båt med i eksisterende bestilling(er).");
        }

        [HttpGet("baater")]
        public async Task<ActionResult> HentAlleBåter()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Baat> alleBåter = await _db.HentAlleBåter();
            if (alleBåter != null)
            {
                _log.LogInformation("AdminController.cs: HentAlleBaater: Vellykket! Båter hentet");
                return Ok(alleBåter);
            }
            _log.LogInformation("AdminController.cs: HentAlleBaater: Databasefeil eller ingen båter i databasen");
            return NotFound("Databasefeil. Ingen båter funnet");
        }

        [HttpGet("baat/{id}")]
        public async Task<ActionResult> HentEnBåt(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            Baat hentetBåt = await _db.HentEnBåt(id);
            if (hentetBåt != null)
            {
                _log.LogInformation("AdminController.cs: HentEnBåt: Vellykket. Båt hentet");
                return Ok(hentetBåt);
            }
            _log.LogInformation("AdminController.cs: HentEnBåt: Ingen båt funnet i databasen med gitt id");
            return NotFound("Båt ikke funnet i databasen");
        }

        [HttpPost("ferder")]
        public async Task<ActionResult> LagreFerd(Ferder ferd)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            if(ModelState.IsValid)
            {
                bool FerdLagret = await _db.LagreFerd(ferd);
                if (FerdLagret)
                {
                    _log.LogInformation("AdminController.cs: LagreFerd: Ferd lagret vellykket");
                    return Ok("Vellykket! Ferd lagret i databasen.");
                }
                _log.LogInformation("AdminController.cs: LagreFerd: Databasefeil eller feil rute/båt-id. Bruker ikke opprettet.");
                return NotFound("Rute eller båt ikke funnet eller databasefeil");
            }
            return BadRequest("Feil i inputvalidering på server");
        }

        [HttpGet("ferd/{id}")]
        public async Task<ActionResult> HentEnFerd(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            Ferder hentetFerd = await _db.HentEnFerd(id);
            if (hentetFerd != null)
            {
                _log.LogInformation("AdminController.cs: HentEnFerd: Vellykket. Ferd hentet");
                return Ok(hentetFerd);
            }
            _log.LogInformation("AdminController.cs: HentEnFerd: Ingen ferd funnet i databasen med gitt id");
            return NotFound("Ingen ferd funnet");
        }

        [HttpGet("ferder")]
        public async Task<ActionResult> HentAlleFerder()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Ferder> alleFerder = await _db.HentAlleFerder();
            if (alleFerder != null)
            {
                _log.LogInformation("AdminController.cs: HentAlleFerder: Vellykket! Ferder hentet");
                return Ok(alleFerder);
            }
            _log.LogInformation("AdminController.cs: HentAlleFerder: Databasefeil. Ferder ikke hentet");
            return new ServiceUnavailableResult("Databasefeil. Ferder ikke hentet");
        }

        [HttpPut("ferd/{id}")]
        public async Task<ActionResult> EndreFerd(Ferder ferd)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            if (ModelState.IsValid)
            {
                bool EndretFerd = await _db.EndreFerd(ferd);
                if (EndretFerd)
                {
                    _log.LogInformation("AdminController.cs: EndreFerd: Vellykket! Ferd endret");
                    return Ok("Vellykket! Ferd endret i databasen");
                }
                _log.LogInformation("AdminController.cs: EndreFerd: Databasefeil. Ferd ikke endret");
                //endres
                return NotFound("Ferd, rute eller båt ikke funnet, ferd med i eksisterende bestilling(er) eller databasefeil");
            }
            _log.LogInformation("AdminController.cs: EndreFerd: Feil i inputvalideringen.");
            return BadRequest("Feil i inputvalidering på server.");
        }

        [HttpDelete("ferd/{id}")]
        public async Task<ActionResult> SlettFerd(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            bool slettet = await _db.SlettFerd(id);
            if (slettet)
            {
                _log.LogInformation("AdminController.cs: SlettFerd: Ferd slettet.");
                return Ok(slettet);
            }
            _log.LogInformation("AdminController.cs: SlettFerd: Ferd med i bestilling(er), eller databasefeil");
            return NotFound("Fant ikke ferb eller ferd med i bestilling(er)");
        }

        [HttpGet("bestillinger")]
        public async Task<ActionResult> HentAlleBestillinger()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Bestilling> alleBestillinger = await _db.HentAlleBestillinger();
            if (alleBestillinger != null)
            {
                _log.LogInformation("AdminController.cs: HentAlleBestillinger: Vellykket! Bestillinger hentet");
                return Ok(alleBestillinger);
            }
            _log.LogInformation("AdminController.cs: HentAlleBestillinger: Databasefeil eller ingen bestillinger i databasen");
            return new ServiceUnavailableResult("Databasefeil. Bestillinger ikke hentet");
        }

        [HttpGet("billetter")]
        public async Task<ActionResult> HentAlleBilletter()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Billetter> alleBilletter = await _db.HentAlleBilletter();
            if (alleBilletter != null)
            {
                _log.LogInformation("AdminController.cs: HentAlleBilletter: Vellykket! Billetter hentet");
                return Ok(alleBilletter);
            }
            _log.LogInformation("AdminController.cs: HentAlleBilletter: Databasefeil. Billetter ikke hentet");
            return new ServiceUnavailableResult("Databasefeil. Billetter ikke hentet");
        }

        [HttpDelete("billett/{id}")]
        public async Task<ActionResult> SlettBillett(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            bool slettet = await _db.SlettBillett(id);
            if (slettet)
            {
                _log.LogInformation("AdminController.cs: SlettBillett: Billett slettet.");
                return Ok(slettet);
            }
            _log.LogInformation("AdminController.cs: SlettFerd: Ferd med i bestilling(er), eller databasefeil");
            return Ok(slettet);
        }

        [HttpGet("billett/{id}")]
        public async Task<ActionResult> HentEnBillett(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            Billetter hentetBillett = await _db.HentEnBillett(id);
            if (hentetBillett != null)
            {
                _log.LogInformation("AdminController.cs: HentEnBillett: Vellykket. Billett hentet");
                return Ok(hentetBillett);
            }
            _log.LogInformation("AdminController.cs: HentEnBillett: Ingen billett funnet i databasen med gitt id");
            return NotFound("Ingen billett funnet");
        }

        [HttpGet("bestilling/{id}/billetter")]
        public async Task<ActionResult> HentBilletterForBestilling(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Billetter> alleBilletter = await _db.HentBilletterForBestilling(id);
            if (alleBilletter != null)
            {
                _log.LogInformation("AdminController.cs: HentBilletterForBestilling: Vellykket! Billetter hentet");
                return Ok(alleBilletter);
            }
            _log.LogInformation("AdminController.cs: HentBilletterForBestilling: Bestilling ikke funnet");
            return NotFound("Bestilling ikke funnet");
        }

        [HttpGet("ferd/{id}/billetter")]
        public async Task<ActionResult> HentBilletterForFerd(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Billetter> alleBilletter = await _db.HentBilletterForFerd(id);
            if (alleBilletter != null)
            {
                _log.LogInformation("AdminController.cs: HentBilletterForFerd: Vellykket! Billetter hentet");
                return Ok(alleBilletter);
            }
            _log.LogInformation("AdminController.cs: HentBilletterForFerd: Databasefeil eller ferd ikke funnet");
            return NotFound("Ferd ikke funnet");
        }

        [HttpGet("rute/{id}/billetter")]
        public async Task<ActionResult> HentBilletterForRute(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Billetter> alleBilletter = await _db.HentBilletterForRute(id);
            if (alleBilletter != null)
            {
                _log.LogInformation("AdminController.cs: HentBilletterForRute: Vellykket! Billetter hentet");
                return Ok(alleBilletter);
            }
            _log.LogInformation("AdminController.cs: HentBilletterForRute: Databasefeil eller rute ikke funnet");
            return NotFound("Rute ikke funnet");
        }

        [HttpGet("kunde/{id}/bestillinger")]
        public async Task<ActionResult> HentBestillingerForKunde(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Bestilling> alleBestillinger = await _db.HentBestillingerForKunde(id);
            if (alleBestillinger!=null)
            {
                _log.LogInformation("AdminController.cs: HentBestillingerForKunde: Vellykket! Bestillinger hentet");
                return Ok(alleBestillinger);
            }
            _log.LogInformation("AdminController.cs: HentBestillingerForKunde: Kunde ikke funnet eller har ikke bestillt reise");
            return NotFound("Kunde ikke funnet");
        }

        [HttpGet("baat/{id}/billetter")]
        public async Task<ActionResult> HentBilletterForBåt(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Billetter> alleBilletter = await _db.HentBilletterForBåt(id);
            if (alleBilletter != null)
            {
                _log.LogInformation("AdminController.cs: HentBilletterForBåt: Vellykket! Billetter hentet");
                return Ok(alleBilletter);
            }
            _log.LogInformation("AdminController.cs: HentBilletterForBåt: Databasefeil eller båt ikke funnet");
            return NotFound("Båt ikke funnet.");
        }

        [HttpGet("kunde/{id}")]
        public async Task<ActionResult> HentEnKunde(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            Kunder kunde = await _db.HentEnKunde(id);
            if (kunde != null)
            {
                _log.LogInformation("AdminController.cs: HentEnKunde: Vellykket! Kunde hentet");
                return Ok(kunde);
            }
            _log.LogInformation("AdminController.cs: HentEnKunde: Kunde ikke funnet");
            return NotFound("Kunde ikke funnet");
        }

        [HttpGet("kunder")]
        public async Task<ActionResult> HentAlleKunder()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Kunder> alleKunder = await _db.HentAlleKunder();
            if (alleKunder != null)
            {
                _log.LogInformation("AdminController.cs: HentAlleKunder: Vellykket! Kunder hentet");
                return Ok(alleKunder);
            }
            _log.LogInformation("AdminController.cs: HentAlleKunder: Databasefeil. Prøv igjen!");
            return new ServiceUnavailableResult("Databasefeil. Ingen kunder hentet");
        }

        [HttpPut("kunde/{id}")]
        public async Task<ActionResult> EndreKunde(Kunder kunde)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            if (ModelState.IsValid)
            {
                bool endretKunde = await _db.EndreKunde(kunde);
                if (endretKunde)
                {
                    _log.LogInformation("AdminController.cs: EndreKunde: Kunde endret i databasen");
                    return Ok("Vellykket! Kunde endret i databasen");
                }
                _log.LogInformation("AdminController.cs: EndreKunde: Databasefeil eller kunde ikke funnet. Ikke endret");
                return NotFound("Kunde ikke funnet i databasen");
            }
            _log.LogInformation("AdminController.cs: EndreKunde: Feil i inputvalideringen. Kunde ikke endret");
            return BadRequest("Feil i inputvalideringen");
        }

        [HttpDelete("kunde/{id}")]
        public async Task<ActionResult> SlettKunde(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            bool slettet = await _db.SlettKunde(id);
            if (slettet)
            {
                _log.LogInformation("AdminController.cs: SlettKunde: Vellykket! Kunde slettet");
                return Ok(slettet);
            }
            _log.LogInformation("AdminController.cs: SlettKunde: Databasefeil, kunde ikke funnet eller ubetalte kundebestillinger i database");
            return NotFound("Kunde ikke funnet eller ubetalte kundebestillinger i databasen");
        }

        [HttpPost("brukere")]
        public async Task<ActionResult> LagreBruker(Bruker bruker)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            if (ModelState.IsValid)
            {
                bool brukerOpprettet = await _db.LagreBruker(bruker);
                if(brukerOpprettet)
                {
                    _log.LogInformation("AdminController.cs: LagreBruker: Bruker lagret vellykket");
                    return Ok(brukerOpprettet);
                }
                _log.LogInformation("AdminController.cs: LagreBruker: Databasefeil. Bruker ikke opprettet.");
                return new ServiceUnavailableResult("Databasefeil. Bruker ikke opprettet");
            }
            return BadRequest("Feil i inputvalidering på server");
        }

        [HttpPost("logg_inn")]
        public async Task<ActionResult> LoggInn(Bruker bruker)
        {
            if (ModelState.IsValid)
            {
                bool loggetInn = await _db.LoggInn(bruker);
                if(loggetInn)
                {
                    _log.LogInformation("AdminController.cs: LoggInn: Bruker logget inn vellykket.");
                    HttpContext.Session.SetString(_loggetInn, "loggetInn");
                    return Ok(loggetInn);
                }
                _log.LogInformation("AdminController.cs: LoggInn: Feil brukernavn eller passord. Ikke logget inn");
                HttpContext.Session.SetString(_loggetInn, "");
                return Ok(loggetInn);
            } 
            _log.LogInformation("AdminController.cs: LoggInn: Feil i inputvalidering for brukernavn og/eller passord");
            return BadRequest("Feil i inputvalideringen på server");
        }

        public void LoggUt()
        {
            HttpContext.Session.SetString(_loggetInn, "");
        }
    }

    public class ServiceUnavailableResult : ViewResult
    {
        public ServiceUnavailableResult(string viewName)
        {
            ViewName = viewName;
            StatusCode = (int)HttpStatusCode.ServiceUnavailable;
        }
    }
}

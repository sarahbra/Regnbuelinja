using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Regnbuelinja.DAL;
using Regnbuelinja.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    return Ok("Vellykket! Rute lagret i databasen");
                } else
                {
                    _log.LogInformation("AdminController.cs: LagreRute: Databasefeil. Rute ikke lagret");
                    return BadRequest("Feil i databasen. Prøv på nytt");
                }
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
                    return Ok("Vellykket! Rute endret i databasen");
                }
                else
                {
                    _log.LogInformation("AdminController.cs: EndreRute: Databasefeil. Rute ikke endret");
                    return NotFound("Rute ikke funnet eller rute med i eksisterende bestilling(er).");
                }
            } else
            {
                _log.LogInformation("AdminController.cs: EndreRute: Feil i inputvalideringen.");
                return BadRequest("Feil i inputvalidering på server.");
            }
            
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
            } else { 
                _log.LogInformation("AdminController.cs: HentAlleRuter: Feil i databasen eller ingen ruter lagret");
                return NotFound("Ingen ruter funnet i databasen.");
            }
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
            else
            {
                _log.LogInformation("AdminController.cs: HentEnRute: Ingen rute funnet i databasen");
                return NotFound("Ingen rute funnet");
            }
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
            } else
            {
                _log.LogInformation("AdminController.cs: SlettRute: Rute finnes ikke eller databasefeil");
                return NotFound("Rute ikke funnet eller rute med i eksisterende bestilling(er).");
            }
            
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
                } else
                {
                    _log.LogInformation("AdminController.cs: LagreBåt: Databasefeil. Båt kunne ikke lagres");
                    return BadRequest("Databasefeil. Båt ikke lagret");
                }
            } else
            {
                _log.LogInformation("AdminController.cs: LagreBåt: Feil i inputvalideringen. Båt ikke lagret");
                return BadRequest("Feil i inputvalideringen");
            }
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
                else
                {
                    _log.LogInformation("AdminController.cs: EndreBåt: Databasefeil. Båt kunne ikke endres");
                    return NotFound("Båt ikke funnet i databasen eller databasefeil.");
                }
            }
            else
            {
                _log.LogInformation("AdminController.cs: EndreBåt: Feil i inputvalideringen. Båt ikke endret");
                return BadRequest("Feil i inputvalideringen");
            }
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
            else
            {
                _log.LogInformation("AdminController.cs: SlettBåt: Databasefeil. Båt kunne ikke endres");
                return NotFound("Båt ikke funnet i databasen eller båt med i eksisterende bestilling(er).");
            }
        }

        [HttpGet("baater")]
        public async Task<ActionResult> HentAlleBåter()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Baat> alleBåter = await _db.HentAlleBåter();
            if (alleBåter.Any())
            {
                _log.LogInformation("AdminController.cs: HentAlleBaater: Vellykket! Båter hentet");
                return Ok(alleBåter);
            }
            else
            {
                _log.LogInformation("AdminController.cs: HentAlleBaater: Databasefeil eller ingen båter i databasen");
                return NotFound("Ingen båter funnet i databasen");
            }
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
            else
            {
                _log.LogInformation("AdminController.cs: HentEnBåt: Ingen båt funnet i databasen med gitt id");
                return NotFound("Ingen båt funnet");
            }
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
                else
                {
                    _log.LogInformation("AdminController.cs: LagreFerd: Databasefeil eller feil rute/båt-id. Bruker ikke opprettet.");
                    return NotFound("Rute eller båt ikke funnet eller databasefeil");
                }
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
            else
            {
                _log.LogInformation("AdminController.cs: HentEnFerd: Ingen ferd funnet i databasen med gitt id");
                return NotFound("Ingen ferd funnet");
            }
        }

        [HttpGet("ferder")]
        public async Task<ActionResult> HentAlleFerder()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Ferder> alleFerder = await _db.HentAlleFerder();
            if (alleFerder.Any())
            {
                _log.LogInformation("AdminController.cs: HentAlleFerder: Vellykket! Ferder hentet");
                return Ok(alleFerder);
            }
            else
            {
                _log.LogInformation("AdminController.cs: HentAlleFerder: Databasefeil eller ingen ferder i databasen");
                return NotFound("Ingen ferder funnet i databasen");
            }
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
                else
                {
                    _log.LogInformation("AdminController.cs: EndreFerd: Databasefeil. Ferd ikke endret");
                    return NotFound("Ferd, rute eller båt ikke funnet, ferd med i eksisterende bestilling(er) eller databasefeil");
                }
            }
            else
            {
                _log.LogInformation("AdminController.cs: EndreFerd: Feil i inputvalideringen.");
                return BadRequest("Feil i inputvalidering på server.");
            }
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
            else
            {
                _log.LogInformation("AdminController.cs: SlettFerd: Ferd med i bestilling(er), eller databasefeil");
                return NotFound("Ferd ikke funnet eller i eksisterende bestilling(er).");
            }

        }

        [HttpGet("bestillinger")]
        public async Task<ActionResult> HentAlleBestillinger()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Bestilling> alleBestillinger = await _db.HentAlleBestillinger();
            if (alleBestillinger.Any())
            {
                _log.LogInformation("AdminController.cs: HentAlleBestillinger: Vellykket! Bestillinger hentet");
                return Ok(alleBestillinger);
            }
            else
            {
                _log.LogInformation("AdminController.cs: HentAlleBestillinger: Databasefeil eller ingen bestillinger i databasen");
                return NotFound("Ingen bestillinger i databasen");
            }
        }

        [HttpGet("billetter")]
        public async Task<ActionResult> HentAlleBilletter()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Billetter> alleBilletter = await _db.HentAlleBilletter();
            if (alleBilletter.Any())
            {
                _log.LogInformation("AdminController.cs: HentAlleBilletter: Vellykket! Billetter hentet");
                return Ok(alleBilletter);
            }
            else
            {
                _log.LogInformation("AdminController.cs: HentAlleBilletter: Databasefeil eller ingen billetter i databasen");
                return NotFound("Ingen billetter i databasen");
            }
        }

        [HttpGet("bestilling/{id}/billetter")]
        public async Task<ActionResult> HentBilletterForBestilling(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Billetter> alleBilletter = await _db.HentBilletterForBestilling(id);
            if (alleBilletter.Any())
            {
                _log.LogInformation("AdminController.cs: HentBilletterForBestilling: Vellykket! Billetter hentet");
                return Ok(alleBilletter);
            }
            else
            {
                _log.LogInformation("AdminController.cs: HentBilletterForBestilling: Databasefeil, ingen billetter i bestilling eller bestilling ikke funnet");
                return NotFound("Ingen billetter i bestilling eller bestilling ikke funnet");
            }
        }

        [HttpGet("ferd/{id}/billetter")]
        public async Task<ActionResult> HentBilletterForFerd(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Billetter> alleBilletter = await _db.HentBilletterForFerd(id);
            if (alleBilletter.Any())
            {
                _log.LogInformation("AdminController.cs: HentBilletterForFerd: Vellykket! Billetter hentet");
                return Ok(alleBilletter);
            }
            else
            {
                _log.LogInformation("AdminController.cs: HentBilletterForFerd: Databasefeil eller ferd ikke funnet");
                return NotFound("Ferd ikke funnet");
            }
        }

        [HttpGet("rute/{id}/billetter")]
        public async Task<ActionResult> HentBilletterForRute(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Billetter> alleBilletter = await _db.HentBilletterForRute(id);
            if (alleBilletter.Any())
            {
                _log.LogInformation("AdminController.cs: HentBilletterForRute: Vellykket! Billetter hentet");
                return Ok(alleBilletter);
            }
            else
            {
                _log.LogInformation("AdminController.cs: HentBilletterForRute: Databasefeil eller rute ikke funnet");
                return NotFound("Rute ikke funnet");
            }
        }

        [HttpGet("baat/{id}/billetter")]
        public async Task<ActionResult> HentBilletterForBåt(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }
            List<Billetter> alleBilletter = await _db.HentBilletterForBåt(id);
            if (alleBilletter.Any())
            {
                _log.LogInformation("AdminController.cs: HentBilletterForBåt: Vellykket! Billetter hentet");
                return Ok(alleBilletter);
            }
            else
            {
                _log.LogInformation("AdminController.cs: HentBilletterForBåt: Databasefeil eller båt ikke funnet");
                return NotFound("Båt ikke funnet");
            }
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
                } else
                {
                    _log.LogInformation("AdminController.cs: LagreBruker: Databasefeil. Bruker ikke opprettet.");
                    return BadRequest("Feil i databasen. Bruker ikke opprettet.");
                }
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
                } else
                {
                    _log.LogInformation("AdminController.cs: LoggInn: Feil brukernavn eller passord. Ikke logget inn");
                    HttpContext.Session.SetString(_loggetInn, "");
                    return Ok(loggetInn);
                }
            } else
            {
                _log.LogInformation("AdminController.cs: LoggInn: Feil i inputvalidering for brukernavn og/eller passord");
                return BadRequest("Feil i inputvalideringen på server");
            }
        }

        public void LoggUt()
        {
            HttpContext.Session.SetString(_loggetInn, "");
        }
    }
}

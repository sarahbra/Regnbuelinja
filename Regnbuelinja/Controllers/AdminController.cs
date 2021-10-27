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
                    return Ok("Feil i databasen. Prøv på nytt");
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
                bool kundeEndret = await _db.EndreRute(rute);
                if (kundeEndret)
                {
                    _log.LogInformation("AdminController.cs: EndreRute: Vellykket! Rute endret");
                    return Ok("Vellykket! Rute endret i databasen");
                }
                else
                {
                    _log.LogInformation("AdminController.cs: EndreRute: Databasefeil. Rute ikke endret");
                    return Ok("Feil i databasen. Prøv på nytt");
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
                return Ok("Feil i databasen. Ingen ruter hentet.");
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

        [HttpDelete("rute{id}")]
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
                return Ok("Rute slettet");
            } else
            {
                _log.LogInformation("AdminController.cs: SlettRute: Rute finnes ikke eller databasefeil");
                return BadRequest("Feil i databasen. Rute ikke slettet");
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
                    return Ok("Båt lagret i databasen");
                } else
                {
                    _log.LogInformation("AdminController.cs: LagreBåt: Databasefeil. Båt kunne ikke lagres");
                    return Ok("Databasefeil. Båt ikke lagret");
                }
            } else
            {
                _log.LogInformation("AdminController.cs: LagreBåt: Feil i inputvalideringen. Båt ikke lagret");
                return BadRequest("Feil i inputvalideringen");
            }
        }

        [HttpPut("baater/{id}")]
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
                    return Ok("Båt endret i databasen");
                }
                else
                {
                    _log.LogInformation("AdminController.cs: EndreBåt: Databasefeil. Båt kunne ikke endres");
                    return NotFound("Båt ikke funnet i databasen");
                }
            }
            else
            {
                _log.LogInformation("AdminController.cs: EndreBåt: Feil i inputvalideringen. Båt ikke endret");
                return BadRequest("Feil i inputvalideringen");
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
                    return Ok("Feil i databasen. Bruker ikke opprettet.");
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

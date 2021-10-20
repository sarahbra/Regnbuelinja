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
    [Route("api/admin/[controller]")]
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

        [HttpPost("/ruter")]
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
                    return Ok("feil i databasen. Prøv på nytt");
                }
            }
            _log.LogInformation("AdminController.cs: LagreRute: Feil i inputvalideringen.");
            return BadRequest("Feil i inputvalideringen.");
        }

        [HttpGet("/ruter")]
        public async Task<ActionResult> HentAlleRuter()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            List<Rute> ruter = await _db.HentAlleRuter();
            if(ruter != null)
            {
                if(!ruter.Any())
                {
                    _log.LogInformation("AdminController.cs: HentAlleRuter: Vellykket. Ingen ruter i databasen");
                    return NotFound("Ingen ruter funnet");
                } else
                {
                    _log.LogInformation("AdminController.cs: HentAlleRuter: Ingen ruter i databasen");
                    return Ok(ruter);
                }
            } else
            {
                _log.LogInformation("AdminController.cs: HentAlleRuter: Feil i databasen. Ingen ruter hentet.");
                return Ok("Feil i databasen. Ingen ruter hentet. Prøv igjen!");
            }
        }

        [HttpPost]
        public async Task<ActionResult> LagreBruker(Bruker bruker)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            if (ModelState.IsValid)
            {
                int brukerOpprettet = await _db.LagreBruker(bruker);
                if(brukerOpprettet!=0)
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

        [HttpGet]
        public async Task<ActionResult> LoggInn(Bruker bruker)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_loggetInn)))
            {
                return Unauthorized("Ikke logget inn");
            }

            if (ModelState.IsValid)
            {
                bool loggetInn = await _db.LoggInn(bruker);
                if(loggetInn)
                {
                    _log.LogInformation("AdminController.cs: LoggInn: Bruker " + bruker.Brukernavn + " logget inn vellykket.");
                    HttpContext.Session.SetString(_loggetInn, "loggetInn");
                    return Ok(loggetInn);
                } else
                {
                    _log.LogInformation("AdminController.cs: LoggInn: Brukernavn eller passord feil. Ikke logget inn");
                    HttpContext.Session.SetString(_loggetInn, "");
                    return NotFound(loggetInn);
                }
            } else
            {
                _log.LogInformation("AdminController.cs: LoggInn: Feil i inputvalidering for brukernavn og/eller passord");
                return BadRequest("Feil i inputvalideringen");
            }
        }
    }
}

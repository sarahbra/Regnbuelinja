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
            if(ModelState.IsValid)
            {
                bool lagret = await _db.LagreRute(rute);
                if(lagret)
                {
                    _log.LogInformation("AdminController.cs: Admin / LagreRute: Vellyket! Rute lagret");
                    return Ok("Vellykket! Rute lagret i databasen");
                } else
                {
                    _log.LogInformation("AdminController.cs: Admin / LagreRute: Databasefeil. Rute ikke lagret");
                    return Ok("Databasefeil. Prøv på nytt");
                }
            }
            _log.LogInformation("AdminController.cs: Admin/LagreRute: Feil i inputvalideringen.");
            return BadRequest("Feil i inputvalideringen.");
        }

        [HttpGet("/ruter")]
        public async Task<ActionResult> HentAlleRuter()
        {
            //List<Rute> ruter = await _db.HentAlleRuter();
            return null;
        }

        [HttpPost]
        public async Task<ActionResult> LagreBruker(Bruker bruker)
        {
            if(ModelState.IsValid)
            {
                bool brukerOpprettet = await _db.LagreBruker(bruker);
                return Ok(brukerOpprettet);
                
            }
            return BadRequest("Feil i inputvalidering på server");
        }

        public async Task<ActionResult> LoggInn(Bruker bruker)
        {
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
                    return Ok(loggetInn);
                }
            } else
            {
                _log.LogInformation("AdminController.cs: LoggInn: Feil i inputvalidering for brukernavn og/eller passord");
                return BadRequest("Feil i inputvalideringen");
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Regnbuelinja.DAL;
using Regnbuelinja.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Regnbuelinja.Controllers
{

    [Route("[controller]/[action]")]
    public class BestillingController : ControllerBase
    {
        private readonly IBestillingRepository _db;

        private ILogger<BestillingController> _log;

        public BestillingController(IBestillingRepository db, ILogger<BestillingController> log)
        {
            _db = db;
            _log = log;
        }

        public async Task<ActionResult> HentAvgangshavner()
        {
            List<string> hentAvgangsHavner = await _db.HentAvgangshavner();
            if (hentAvgangsHavner == null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentAvgangshavner: Avgangshavnene ble ikke returnert.");
                return NotFound("Finner ikke avgangshavner i repository");
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentAvgangshavner: Vellykket. Avgangshavnene har blitt returnert.");
            return Ok(hentAvgangsHavner);
        }

        public async Task<ActionResult> HentAnkomsthavner(string avgangsHavn)
        {
            List<string> hentAnkomstHavner = await _db.HentAnkomsthavner(avgangsHavn);
            if (hentAnkomstHavner == null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentAnkomsthavner: Amkomsthavnene ble ikke returnert.");
                NotFound("Finner ikke ankomsthavner i repository");
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentAnkomsthavner: Vellykket. Ankomsthavnene har blitt returnert.");
            return Ok(hentAnkomstHavner);
        }

        //vet ikke om trengs. Kan nok fjernes
        public async Task<ActionResult> HentRuter(string nyttstartpunkt)
        {
            List<Rute> hentruter = await _db.HentRuter(nyttstartpunkt);
            if (hentruter == null)
            {
                return NotFound("finner ikke ruter i repository");
            }
            return Ok(hentruter);
        }

        //samme gjelder denne
        public async Task<ActionResult> HentFerder(int ruteid)
        {
            List<Ferd> hentferder = await _db.HentFerder(ruteid);
            if (hentferder == null)
            {
                return NotFound("finner ikke ferder i repository");
            }
            return Ok(hentferder);
        }

        //BestillingInput, i stedetfor Bestilling og FromBody fordi vi vil sende inn et JSON-objekt
        public async Task<ActionResult> LagreBestilling(BestillingInput nyBestilling)
        {
            if (ModelState.IsValid)
            {
                string lagreBestilling = await _db.LagreBestilling(nyBestilling);
                if (lagreBestilling == null)
                {
                    _log.LogInformation("/Controllers/BestillingController.cs: LagreBestilling: Kunne ikke lagre bestilling i databasen (sannsynligvis i BestillingRepository.cs)");
                    return BadRequest("Kunne ikke lagre bestillingen i databasen");
                }
                _log.LogInformation("/Controllers/BestillingController.cs: LagreBestilling: Vellykket. Bestillingen har blitt lagret.");
                return Ok(lagreBestilling);
            }
            _log.LogInformation("/Controllers/BestillingController.cs: LagreBestilling: Invalid client input.");
            return ValidationProblem("Invalid input object from client");
        }

        public async Task<ActionResult> HentBestilling(int id)
        {
            BestillingInput hentBestilling = await _db.HentBestilling(id);
            if (hentBestilling == null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentBestilling: Ingen bestilling med ID " + id + " har blitt funnet.");
                return BadRequest("Ingen bestilling med ID " + id + " har blitt funnet.");           
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentBestilling: Vellykket. Bestilling med ID " + id + " har blitt funnet.");
            return Ok(hentBestilling);
        }

        public async Task<ActionResult> HentDatoer(string Startpunkt, string Endepunkt, string AvreiseTid)
        {
            List<DateTime> Datoer = await _db.HentDatoer(Startpunkt, Endepunkt, AvreiseTid);
            return Ok(Datoer);
        }

        public async Task<ActionResult> HentPris(int id)
        {
            double TotalPris = await _db.HentPris(id);
            return Ok(TotalPris);
        }

        public async Task<ActionResult> HentAnkomstTid(int id, string Startpunkt)
        {
            string AnkomstTid = await _db.HentAnkomstTid(id, Startpunkt);
            if (AnkomstTid == null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentAnkomstTid: Ingen ankomsttid funnet for bestilling " + id + " fra startpunktet Startpunkt");
                return BadRequest("Ingen ankomsttid funnet for bestilling " + id + " for startpunktet " + Startpunkt);
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentAnkomstTid: Vellykket. Amkosttid(er) har blitt funnet for bestilling " + id + " fra startpunkt " + Startpunkt);
            return Ok(AnkomstTid);
        }
    }
}
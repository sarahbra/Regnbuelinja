﻿using Microsoft.AspNetCore.Mvc;
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

        public async Task<ActionResult> HentBaat(int id, string Startpunkt)
        {
            string Baatnavn = await _db.HentBaat(id, Startpunkt);
            if (Baatnavn == null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentBåt: Båtnavn ble ikke returnert.");
                return NotFound("Finner ikke båtnavn i repository");
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentBåt: Vellykket. Båtnavn har blitt returnert.");
            return Ok(Baatnavn);
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

        public async Task<ActionResult> HentRuter(string nyttstartpunkt)
        {
            List<Rute> hentruter = await _db.HentRuter(nyttstartpunkt);
            if (hentruter == null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentRuter: Rutene har ikke blitt hentet fra databasen.");
                return NotFound("finner ikke ruter i repository");
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentRuter: Vellykket. Rutene har blitt hentet fra databasen.");
            return Ok(hentruter);
        }

        public async Task<ActionResult> HentFerder(int ruteid)
        {
            List<Ferd> hentferder = await _db.HentFerder(ruteid);
            if (hentferder == null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentFerder: Ingen ferder har blitt hentet fra databasen.");
                return NotFound("finner ikke ferder i repository");
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentFerder: Vellykket. Ferdene har blitt hentet fra databasen.");
            return Ok(hentferder);
        }

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
            if (Datoer == null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentDatoer: Ingen datoer har blitt hentet fra databasen.");
                return BadRequest("Ingen datoer har blitt funnet.");
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentDatoer: Vellykket. Datoene har blitt hentet fra databasen.");
            return Ok(Datoer);
        }

        public async Task<ActionResult> HentPris(int id)
        {
            double TotalPris = await _db.HentPris(id);
            if (TotalPris == 0)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentPris: TotalPris er lik 0. Dette kan bety at ingen pris har blitt hentet fra databasen.");
            }
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
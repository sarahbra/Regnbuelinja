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

        public async Task<ActionResult> HentBaat(int id, string Startpunkt)
        {
            string Baatnavn = await _db.HentBaat(id, Startpunkt);
            if (Baatnavn == null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentBåt: Båtnavn ble ikke returnert.");
                return NotFound("Finner ikke båtnavn i repository for bestilling med id " + id);
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

        public async Task<ActionResult> LagreBestilling(BestillingOutput nyBestilling)
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

        public async Task<ActionResult> HentAlleBestillingerForKunde(int id)
        {
            List<Bestilling> bestillinger = await _db.HentBestillingerForKunde(id);
            if(bestillinger != null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentAlleBestillingerForKunde: Vellykket! Bestillinger hentet");
                return Ok(bestillinger);
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentAlleBestillingerForKunde: Kunde ikke funnet i databasen");
            return NotFound("Ingen bestillinger for kunde med ID " + id + " funnet");
        }


        public async Task<ActionResult> HentBestilling(int id)
        {
            BestillingOutput hentBestilling = await _db.HentBestilling(id);
            if (hentBestilling == null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentBestilling: Ingen bestilling med ID " + id + " har blitt funnet.");
                return NotFound("Ingen bestilling med ID " + id + " har blitt funnet i databasen.");
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentBestilling: Vellykket. Bestilling med ID " + id + " har blitt funnet.");
            return Ok(hentBestilling);
        }

        public async Task<ActionResult> HentDatoer(string Startpunkt, string Endepunkt, string AvreiseTid)
        {
            List<DateTime> Datoer = await _db.HentDatoer(Startpunkt, Endepunkt, AvreiseTid);
            if (Datoer == null)
            {
                DateTime AvreiseDatoTid = DateTime.Parse(AvreiseTid);
                var AvreiseDato = AvreiseDatoTid.ToString("dd/MM/yyyy");

                _log.LogInformation("/Controllers/BestillingController.cs: HentDatoer: Ingen returdatoer funnet for avreisedato " + AvreiseTid + " med avreisehavn " + Startpunkt + " og ankomsthavn " + Endepunkt);
                return NotFound("Ingen returdato med avreisedato " + AvreiseDato + " fra " + Startpunkt + " til " + Endepunkt + " funnet. Velg en tidligere avreisedato.");
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentBestilling: Vellykket. Returdatoer for avreisedato " + AvreiseTid + " fra " + Startpunkt + " til " + Endepunkt + " har blitt funnet.");
            return Ok(Datoer);
        }

        public async Task<ActionResult> HentPris(int id)
        {
            double TotalPris = await _db.HentPris(id);
            if (TotalPris == default)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentPris: Ingen totalpris funnet for bestilling med id " + id);
                return NotFound("Bestilling med id " + id + " er ikke registrert med totalpris eller finnes ikke i databasen");
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentPris: Vellykket. Totalpris for bestilling med id " + id + " har blitt funnet.");
            return Ok(TotalPris);
        }

        public async Task<ActionResult> HentAnkomstTid(int id, string Startpunkt)
        {
            string AnkomstTid = await _db.HentAnkomstTid(id, Startpunkt);
            if (AnkomstTid == null)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentAnkomstTid: Ingen ankomsttid funnet for bestilling " + id + " fra avreisehavn " + Startpunkt);
                return NotFound("Ingen ankomsttid funnet for bestilling " + id + " for avreisehavn " + Startpunkt);
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentAnkomstTid: Vellykket. Amkosttid(er) har blitt funnet for bestilling " + id + " fra avreisehavn " + Startpunkt);
            return Ok(AnkomstTid);
        }

        public async Task<ActionResult> HentStrekningsPris(int id, string Startpunkt)
        {
            double StrekningsPris = await _db.HentStrekningsPris(id, Startpunkt);
            if(StrekningsPris == 0.00)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: HentStrekningsPris: Ingen strekningspris funnet for bestilling " + id + " fra avreisehavn " + Startpunkt);
                return NotFound("Ingen ankomsttid funnet for bestilling " + id + " for avreisehavn " + Startpunkt);
            }
            _log.LogInformation("/Controllers/BestillingController.cs: HentStrekningsPris: Vellykket. Strekningspris har blitt funnet for bestilling " + id + " fra avreisehavn " + Startpunkt);
            return Ok(StrekningsPris);
        }

        public async Task<ActionResult> LagreKunde(Personer kunde)
        {
            if(ModelState.IsValid)
            {
                int kundeLagret = await _db.LagreKunde(kunde);
                if(kundeLagret != 0)
                {
                    _log.LogInformation("/Controllers/BestillingController.cs: LagreKunde: Vellykket. Kunde lagret i databasen");
                    return Ok(kundeLagret);
                }
                _log.LogInformation("/Controllers/BestillingController.cs: LagreKunde: Feil i databasen. Prøv igjen!");
                return Ok(kundeLagret);
            }
            _log.LogInformation("/Controllers/BestillingController.cs: LagreKunde: Feil i inputvalideringen");
            return BadRequest("Feil i inputvalideringen");
        }

        public async Task<ActionResult> LeggKundeTilBestilling(int BId, int KId)
        {
            bool lagtTil = await _db.LeggKundeTilBestilling(BId, KId);
            if(lagtTil)
            {
                _log.LogInformation("/Controllers/BestillingController.cs: LeggKundeTilBestilling: Vellykket. Kunde lagt til bestillingen");
                return Ok(lagtTil);
            }
            _log.LogInformation("/Controllers/BestillingController.cs: LeggKundeTilBestilling: Fant ikke kunde eller bestilling");
            return NotFound("Fant ikke kunde eller bestilling i databasen");
        }
    }
}
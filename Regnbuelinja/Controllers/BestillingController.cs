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
            _log.LogInformation("------");
            List<string> hentAvgangsHavner = await _db.HentAvgangshavner();
            if (hentAvgangsHavner == null)
            {
                return NotFound("Finner ikke avgangshavner i repository");
            }
            return Ok(hentAvgangsHavner);
        }

        public async Task<ActionResult> HentAnkomsthavner(string avgangsHavn)
        {
            List<string> hentAnkomstHavner = await _db.HentAnkomsthavner(avgangsHavn);
            if (hentAnkomstHavner == null)
            {
                NotFound("Finner ikke ankomsthavner i repository");
            }
            return Ok(hentAnkomstHavner);
        }

        //vet ikke om trengs. Kan nok fjernes
        public async Task<ActionResult> HentRuter(string nyttStartPunkt)
        {
            List<Rute> hentRuter = await _db.HentRuter(nyttStartPunkt);
            if (hentRuter == null)
            {
                return NotFound("Finner ikke ruter i repository");
            }
            return Ok(hentRuter);
        }

        //Samme gjelder denne
        public async Task<ActionResult> HentFerder(int ruteId)
        {
            List<Ferd> hentFerder = await _db.HentFerder(ruteId);
            if (hentFerder == null)
            {
                return NotFound("Finner ikke ferder i repository");
            }
            return Ok(hentFerder);
        }

        //BestillingInput, i stedetfor Bestilling og FromBody fordi vi vil sende inn et JSON-objekt
        public async Task<ActionResult> LagreBestilling(BestillingInput nyBestilling)
        {
            string lagreBestilling = await _db.LagreBestilling(nyBestilling);
            if (lagreBestilling.Equals(null))
            {
                return BadRequest("Kunne ikke lagre kunden i repository");
            }
            return Ok(lagreBestilling);
        }

        public async Task<ActionResult> HentBestilling(int id)
        {
            BestillingInput hentBestilling = await _db.HentBestilling(id);
            return Ok(hentBestilling);
        }
    }
}
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

        public async Task<List<string>> HentAvgangshavner()
        {
            _log.LogInformation("------");
            return await _db.HentAvgangshavner();
        }

        public async Task<double> HentPris(int id)
        {
            return await _db.HentPris(id);
        }

        public async Task<List<string>> HentAnkomsthavner(string avgangsHavn)
        {
            return await _db.HentAnkomsthavner(avgangsHavn);
        }

        //vet ikke om trengs. Kan nok fjernes
        public async Task<List<Rute>> HentRuter(string nyttStartPunkt)
        {
            return await _db.HentRuter(nyttStartPunkt);
        }

        //Samme gjelder denne
        public async Task<List<Ferd>> HentFerder(int ruteId)
        {
            return await _db.HentFerder(ruteId);
        }

        //BestillingInput, i stedetfor Bestilling og FromBody fordi vi vil sende inn et JSON-objekt
        public async Task<string> LagreBestilling(BestillingInput nyBestilling)
        {
            return await _db.LagreBestilling(nyBestilling);
        }

        public async Task<BestillingInput> HentBestilling(int id)
        {
            return await _db.HentBestilling(id);
        }
    }
}
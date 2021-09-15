using Microsoft.AspNetCore.Mvc;
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

        public BestillingController(IBestillingRepository db)
        {
            _db = db;
        }

        public async Task<List<Rute>> HentRuter(string nyttStartPunkt)
        {
            return await _db.HentRuter(nyttStartPunkt);
        }

        public async Task<List<Ferd>> HentFerder(int ruteId)
        {
            return await _db.HentFerder(ruteId);
        }

        //BestillingInput, i stedetfor Bestilling og FromBody fordi vi vil sende inn et JSON-objekt
        public async Task LagreBestilling(BestillingInput nyBestilling)
        {
            await _db.LagreBestilling(nyBestilling);
        }

        public async Task<BestillingInput> HentBestilling(int id)
        {
            return await _db.HentBestilling(id);
        }
    }
}
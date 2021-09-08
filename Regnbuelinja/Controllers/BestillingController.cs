using Microsoft.AspNetCore.Mvc;
using Regnbuelinja.DAL;
using Regnbuelinja.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task LagreBestilling(Bestilling nyBestilling)
        {
            await _db.LagreBestilling(nyBestilling);
        }

        public async Task<Bestilling> HentBestilling(int id)
        {
            return await _db.HentBestilling(id);
        }


    }
}

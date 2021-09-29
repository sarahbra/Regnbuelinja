using Regnbuelinja.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Regnbuelinja.DAL
{
    public interface IBestillingRepository
    {
        Task<ActionResult> HentAvgangshavner();
        Task<List<string>> HentAnkomsthavner(string avgangsHavn);
        Task<List<Rute>> HentRuter(string nyttStartPunkt);
        Task<List<Ferd>> HentFerder(int ruteId);
        Task<string> LagreBestilling(BestillingInput nyBestilling);
        Task<BestillingInput> HentBestilling(int id);

    }
}

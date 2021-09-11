using Regnbuelinja.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Regnbuelinja.DAL
{
    public interface IBestillingRepository
    {
        Task<List<Rute>> HentRuter(string nyttStartPunkt);
        Task<List<Ferd>> HentFerder(int ruteId);
        Task LagreBestilling(BestillingInput nyBestilling);
        Task<BestillingInput> HentBestilling(int id);

    }
}

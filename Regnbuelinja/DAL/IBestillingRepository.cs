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
        Task<List<string>> HentAvgangshavner();
        Task<double> HentPris(int id);
        Task<List<DateTime>> HentDatoer(string Startpunkt, string Endepunkt, DateTime AvreiseDato);
        Task<List<string>> HentAnkomsthavner(string avgangsHavn);
        Task<List<Rute>> HentRuter(string nyttStartPunkt);
        Task<List<Ferd>> HentFerder(int ruteId);
        Task<string> LagreBestilling(BestillingInput nyBestilling);
        Task<BestillingInput> HentBestilling(int id);
        Task<string> HentAnkomstTid(string avreiseTid);

    }
}

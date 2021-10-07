using Regnbuelinja.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Regnbuelinja.DAL
{
    public interface IBestillingRepository
    {
        Task<List<string>> HentAvgangshavner();
        Task<double> HentPris(int id);
        Task<List<DateTime>> HentDatoer(string Startpunkt, string Endepunkt, string AvreiseTid);
        Task<string> HentBaat(int id, string Startpunkt);
        Task<List<string>> HentAnkomsthavner(string avgangsHavn);
        Task<List<Rute>> HentRuter(string nyttStartPunkt);
        Task<List<Ferd>> HentFerder(int ruteId);
        Task<string> LagreBestilling(Bestilling nyBestilling);
        Task<Bestilling> HentBestilling(int id);
        Task<string> HentAnkomstTid(int id, string Startpunkt);
        Task<double> HentStrekningsPris(int id, string Startpunkt);

    }
}

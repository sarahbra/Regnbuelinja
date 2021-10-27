using Regnbuelinja.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Regnbuelinja.DAL
{
    public interface IBestillingRepository
    {
        Task<bool> EndreRute(Ruter rute);
        Task<bool> LagreRute(Ruter rute);
        Task<List<Rute>> HentAlleRuter();
        Task<Rute> HentEnRute(int id);
        Task<bool> SlettRute(int id);
        Task<bool> LagreBåt(Baater båt);
        Task<bool> EndreBåt(Baater båt);
        Task<bool> SlettBåt(int id);
        Task<List<Baat>> HentAlleBåter();
        Task<Baat> HentEnBåt(int id);
        Task<bool> LagreFerd(Ferder ferd);
        Task<bool> LagreBruker(Bruker bruker);
        Task<bool> LoggInn(Bruker bruker);
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

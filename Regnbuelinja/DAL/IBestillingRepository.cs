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
        Task<List<Ferder>> HentAlleFerder();
        Task<Ferder> HentEnFerd(int id);
        Task<bool> EndreFerd(Ferder ferd);
        Task<bool> SlettFerd(int id);
        Task<List<Bestilling>> HentAlleBestillinger();
        Task<Bestilling> HentEnBestilling(int id);
        Task<bool> SlettBestilling(int id);
        Task<bool> EndreBestilling(Bestilling bestilling);
        Task<Billetter> HentEnBillett(int id);
        Task<bool> EndreBillett(int id);
        Task<bool> SlettBillett(int id);
        Task<List<Billetter>> HentAlleBilletter();
        Task<List<Billetter>> HentBilletterForBestilling(int id);
        Task<List<Billetter>> HentBilletterForFerd(int id);
        Task<List<Billetter>> HentBilletterForRute(int id);
        Task<List<Billetter>> HentBilletterForBåt(int id);
        Task<List<Bestilling>> HentBestillingerForKunde(int id);
        Task<bool> LagreBruker(Bruker bruker);
        Task<bool> LagreKunde(Kunder kunde);
        Task<bool> EndreKunde(Kunder kunde);
        Task<bool> SlettKunde(int id);
        Task<Kunder> HentEnKunde(int id);
        Task<List<Kunder>> HentAlleKunder();
        Task<bool> LoggInn(Bruker bruker);
        Task<List<string>> HentAvgangshavner();
        Task<double> HentPris(int id);
        Task<List<DateTime>> HentDatoer(string Startpunkt, string Endepunkt, string AvreiseTid);
        Task<string> HentBaat(int id, string Startpunkt);
        Task<List<string>> HentAnkomsthavner(string avgangsHavn);
        Task<List<Rute>> HentRuter(string nyttStartPunkt);
        Task<List<Ferd>> HentFerder(int ruteId);
        Task<string> LagreBestilling(BestillingOutput nyBestilling);
        Task<BestillingOutput> HentBestilling(int id);
        Task<bool> Betal(int id);
        Task<string> HentAnkomstTid(int id, string Startpunkt);
        Task<double> HentStrekningsPris(int id, string Startpunkt);
    }
}

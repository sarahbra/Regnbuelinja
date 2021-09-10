using Microsoft.EntityFrameworkCore;
using Regnbuelinja.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Regnbuelinja.DAL
{
    public class BestillingRepository : IBestillingRepository
    {
        private readonly BestillingContext _db;

        public BestillingRepository(BestillingContext db)
        {
            _db = db;
        }

        public async Task<List<Rute>> HentRuter(string nyttStartPunkt)
        {
            List<Rute> ruter = await _db.Ruter.Where(r => r.Startpunkt.Equals(nyttStartPunkt)).ToListAsync();
            return ruter;
        }

        public async Task<List<Ferd>> HentFerder(int ruteId)
        {
            List<Ferd> ferder = await _db.Ferder.Where(f => f.Rute.RId == ruteId).ToListAsync();
            return ferder;
        }

        public async Task LagreBestilling(BestillingInput nyBestilling)
        {
            double totalPris = 0.00;
            List<Billett> billettListe = new List<Billett>();

            // TODO: Lag flere billetter basert på nyBestilling.TurRetur

            Ferd ferd = await _db.Ferder.FirstOrDefaultAsync(f => f.Dato.Equals(nyBestilling.Dato) &&
                f.Rute.Startpunkt.Equals(nyBestilling.Startpunkt) && f.Rute.Endepunkt.Equals(nyBestilling.Endepunkt));

            for(int i=1; i<= nyBestilling.AntallVoksne; i++)
            {
                Billett nyBillett = new Billett()
                {
                    Ferd = ferd,
                    Voksen = true,
                };
                billettListe.Add(nyBillett);
                totalPris += ferd.Rute.Pris;
            }

            for(int i=1; i<= nyBestilling.AntallBarn; i++)
            {
                Billett nyBillett = new Billett()
                {
                    Ferd = ferd,
                    Voksen = false,
                };
                billettListe.Add(nyBillett);
                totalPris += (ferd.Rute.Pris * 0.5);
            }

            Bestillinger bestilling = new Bestillinger()
            {
                TotalPris = totalPris,
                Billetter = billettListe
            };

            await _db.SaveChangesAsync();
        }

        public async Task<Bestilling> HentBestilling(int id)
        {
            Bestillinger bestillingIDB = await _db.Bestillinger.FirstOrDefaultAsync(b => b.BeId == id);
            Ferd ferden = bestillingIDB.Billetter.First().Ferd;
            int antallVoksne = bestillingIDB.Billetter.Count(b => b.Voksen == true);
            int antallBarn = (bestillingIDB.Billetter.Count() - antallVoksne);
            Bestilling bestilling = new Bestilling()
            {
                Id = bestillingIDB.BeId,
                Startpunkt = ferden.Rute.Startpunkt,
                Endepunkt = ferden.Rute.Endepunkt,
                Dato = ferden.Dato,
                AntallVoksne = antallVoksne,
                AntallBarn = antallBarn,
                Båtnavn = ferden.Båt.Navn,
                TotalPris = bestillingIDB.TotalPris
            };
            return bestilling;
        }
    }
}

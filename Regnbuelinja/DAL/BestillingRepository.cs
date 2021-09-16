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

            Ferd ferd = await _db.Ferder.FirstOrDefaultAsync(f => f.Dato.Equals(nyBestilling.AvreiseDato) &&
                f.Rute.Startpunkt.Equals(nyBestilling.Startpunkt) && f.Rute.Endepunkt.Equals(nyBestilling.Endepunkt));

            // finner ferden som er tilpasset til retur datoen og retur ruten (hvis bestillingen er tur/retur)
            Ferd ferdRetur;
            if (nyBestilling.HjemreiseDato != null)
            {
                ferdRetur = await _db.Ferder.FirstOrDefaultAsync(f => f.Dato.Equals(nyBestilling.HjemreiseDato) &&
                  f.Rute.Startpunkt.Equals(nyBestilling.Endepunkt) && f.Rute.Endepunkt.Equals(nyBestilling.Startpunkt));
            }
            else
            {
                ferdRetur = null;
            }

            if (ferd != null)
            {
                Billett nyBillett;
                for (int i = 1; i <= nyBestilling.AntallVoksne; i++)
                {
                    nyBillett = new Billett()
                    {
                        Ferd = ferd,
                        Voksen = true,
                    };
                    billettListe.Add(nyBillett);
                    totalPris += ferd.Rute.Pris;

                    if (ferdRetur != null)
                    {
                        Billett returBillett = new Billett()
                        {
                            Ferd = ferdRetur,
                            Voksen = true
                        };
                        billettListe.Add(returBillett);
                        totalPris += ferdRetur.Rute.Pris;
                    }
                }

                for (int i = 1; i <= nyBestilling.AntallBarn; i++)
                {
                    nyBillett = new Billett()
                    {
                        Ferd = ferd,
                        Voksen = false,
                    };
                    billettListe.Add(nyBillett);
                    totalPris += (ferd.Rute.Pris * 0.5);

                    // Leger til en retur billetten

                    if (ferdRetur != null)
                    {
                        Billett returBillett = new Billett()
                        {
                            Ferd = ferdRetur,
                            Voksen = true
                        };
                        billettListe.Add(returBillett);
                        totalPris += ferdRetur.Rute.Pris * 0.5;
                    }

                }
            }

            //Oppretter bestillingen
            Bestillinger bestilling = new Bestillinger()
            {
                TotalPris = totalPris,
                Billetter = billettListe
            };


            _db.Bestillinger.Add(bestilling);
            await _db.SaveChangesAsync();
        }

        public async Task<BestillingInput> HentBestilling(int id)
        {
            Bestillinger bestillingIDB = await _db.Bestillinger.FirstOrDefaultAsync(b => b.BeId == id);
            Ferd ferden = bestillingIDB.Billetter.First().Ferd;
            Billett returBillett = bestillingIDB.Billetter.FirstOrDefault(b => b.Ferd.Dato != ferden.Dato);
            Ferd returFerden;
            string hjemreiseDato = null;
            if(returBillett!= default(Billett))
            {
                returFerden = returBillett.Ferd;
                hjemreiseDato = returFerden.Dato;
            }
      
            int antallVoksne = bestillingIDB.Billetter.Count(b => b.Voksen == true);
            int antallBarn = (bestillingIDB.Billetter.Count() - antallVoksne);


            BestillingInput bestilling = new BestillingInput()
            {
                Startpunkt = ferden.Rute.Startpunkt,
                Endepunkt = ferden.Rute.Endepunkt,
                AvreiseDato = ferden.Dato,
                HjemreiseDato = hjemreiseDato,
                AntallVoksne = antallVoksne,
                AntallBarn = antallBarn
            };

            return bestilling;
        }
    }
}
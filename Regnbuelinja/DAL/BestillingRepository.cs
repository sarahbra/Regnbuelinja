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

        public async Task<List<string>> HentAvgangshavner()
        {
            List<string> havner = await _db.Ruter.Select(r => r.Startpunkt).Distinct().ToListAsync();
            return havner;
        }
        public async Task<List<string>> HentAnkomsthavner(string avgangsHavn)
        {
            List<string> havner = await _db.Ruter.Where(r => r.Startpunkt.Equals(avgangsHavn)).Select(r => r.Endepunkt).ToListAsync();
            return havner;
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

        public async Task<string> LagreBestilling(BestillingInput nyBestilling)
        {
            double totalPris = 0.00;
            List<Billett> billettListe = new List<Billett>();

            // TODO: Lag flere billetter basert på nyBestilling.TurRetur

            Ferd ferd = await _db.Ferder.FirstOrDefaultAsync(f => f.Dato.Equals(nyBestilling.AvreiseDato) &&
                f.Rute.Startpunkt.Equals(nyBestilling.Startpunkt) && f.Rute.Endepunkt.Equals(nyBestilling.Endepunkt));

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
            return bestilling.BeId + "";
        }

        public async Task<BestillingInput> HentBestilling(int id)
        {
            BestillingInput bestilling = await _db.Bestillinger.Where(b => b.BeId == id).Select(b => new BestillingInput
            {
                Startpunkt = b.Billetter.First().Ferd.Rute.Startpunkt,
                Endepunkt = b.Billetter.First().Ferd.Rute.Endepunkt,
                AvreiseDato = b.Billetter.First().Ferd.Dato,
                HjemreiseDato = b.Billetter.FirstOrDefault(bi => bi.Ferd.FId != b.Billetter.First().Ferd.FId).Ferd.Dato,
                //TODO: Disse returnerer totalt antall billetter og IKKE antall voksen og antall barn
                AntallVoksne = b.Billetter.Where(bi => bi.Ferd.Dato.Equals(b.Billetter.First().Ferd.Dato) && bi.Voksen == true).Count(),
                AntallBarn = b.Billetter.Where(bi => bi.Ferd.Dato.Equals(b.Billetter.First().Ferd.Dato) && bi.Voksen == false).Count()
            }).FirstOrDefaultAsync();

            return bestilling;
        }

        public async Task<double> HentPris(int id)
        {
            double pris = await _db.Bestillinger.Where(b => b.BeId == id).Select(b => b.TotalPris).FirstOrDefaultAsync();
            return pris;
        }

        public async Task<List<DateTime>> HentDatoer(string Startpunkt, string Endepunkt, DateTime AvreiseDato)
        {
            List<DateTime> Datoer;
            if(AvreiseDato == null)
            {
                Datoer = await _db.Ferder.Where(f => (f.Rute.Startpunkt.Equals(Startpunkt) && (f.Rute.Endepunkt.Equals(Endepunkt)))).Select(f => f.Dato).ToListAsync();
            } else
            {
                Datoer = await _db.Ferder.Where(f => (f.Rute.Startpunkt.Equals(Startpunkt) && (f.Rute.Endepunkt.Equals(Endepunkt))) && f.Dato.CompareTo(AvreiseDato) > 0).Select(f => f.Dato).ToListAsync();
            }
            Datoer.Sort();
            return Datoer;
        }
    }
}
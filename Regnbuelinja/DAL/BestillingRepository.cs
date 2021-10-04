using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        private ILogger<BestillingRepository> _log;

        public BestillingRepository(BestillingContext db, ILogger<BestillingRepository> log)
        {
            _db = db;
            _log = log;
        }

        public async Task<List<string>> HentAvgangshavner()
        {
            List<string> havner = await _db.Ruter.Select(r => r.Startpunkt).Distinct().ToListAsync();
            if (havner != null)
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentAvgangshavner: Vellykket. Avgangshavnene ble returnert fra databasen.");
                return havner;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentAvgangshavner: Avgangshavnene ble ikke returnert fra databasen.");
            return havner;
        }
        public async Task<List<string>> HentAnkomsthavner(string avgangsHavn)
        {
            List<string> havner = await _db.Ruter.Where(r => r.Startpunkt.Equals(avgangsHavn)).Select(r => r.Endepunkt).ToListAsync();
            if (havner != null)
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentAnkomsthavner: Vellykket. Ankomsthavnene ble returnert fra databasen.");
                return havner;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentAnkomsthavner: Ankomstshavnene ble ikke returnert fra databasen.");
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

            System.Diagnostics.Debug.WriteLine(nyBestilling.Startpunkt);
            System.Diagnostics.Debug.WriteLine(nyBestilling.Endepunkt);
            System.Diagnostics.Debug.WriteLine(nyBestilling.AvreiseTid);
            System.Diagnostics.Debug.WriteLine(nyBestilling.HjemreiseTid);
            System.Diagnostics.Debug.WriteLine(nyBestilling.AntallVoksne);
            System.Diagnostics.Debug.WriteLine(nyBestilling.AntallBarn);
            Ferd ferd = await _db.Ferder.FirstOrDefaultAsync(f => f.AvreiseTid.Equals(nyBestilling.AvreiseTid) &&
                f.Rute.Startpunkt.Equals(nyBestilling.Startpunkt) && f.Rute.Endepunkt.Equals(nyBestilling.Endepunkt));

            Ferd ferdRetur;
            if (nyBestilling.HjemreiseTid != null)
            {
                ferdRetur = await _db.Ferder.FirstOrDefaultAsync(f => f.AvreiseTid.Equals(nyBestilling.HjemreiseTid) &&
                  f.Rute.Startpunkt.Equals(nyBestilling.Endepunkt) && f.Rute.Endepunkt.Equals(nyBestilling.Startpunkt));
            }
            else
            {
                ferdRetur = null;
            }

            if (ferd != null)
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: LagreBestilling: Ferd med passende dato, og de samme start- og endepunktene har blitt funnet i databasen.");
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

                    // Legger til retur billetten
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
            else
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: LagreBestilling: Ferd med passende dato, og de samme start- og endepunktene har ikke blitt funnet i databasen.");
                return null;
            }
        }

        public async Task<BestillingInput> HentBestilling(int id)
        {
            BestillingInput bestilling = await _db.Bestillinger.Where(b => b.BeId == id).Select(b => new BestillingInput
            {
                Startpunkt = b.Billetter.First().Ferd.Rute.Startpunkt,
                Endepunkt = b.Billetter.First().Ferd.Rute.Endepunkt,
                AvreiseTid = b.Billetter.First().Ferd.AvreiseTid,
                HjemreiseTid = b.Billetter.FirstOrDefault(bi => bi.Ferd.FId != b.Billetter.First().Ferd.FId).Ferd.AvreiseTid,
                AntallVoksne = b.Billetter.Where(bi => bi.Ferd.AvreiseTid.Equals(b.Billetter.First().Ferd.AvreiseTid) && bi.Voksen == true).Count(),
                AntallBarn = b.Billetter.Where(bi => bi.Ferd.AvreiseTid.Equals(b.Billetter.First().Ferd.AvreiseTid) && bi.Voksen == false).Count()
            }).FirstOrDefaultAsync();

            if (bestilling != null)
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentBestilling: Vellykket. Et BestillingInput objekt blir returnert.");
                return bestilling;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentBestilling: Ingen bestilling med ID " + id + " har blitt funnet i databasen");
            return bestilling;
        }

        public async Task<double> HentPris(int id)
        {
            double pris = await _db.Bestillinger.Where(b => b.BeId == id).Select(b => b.TotalPris).FirstOrDefaultAsync();
            return pris;
        }

        public async Task<List<DateTime>> HentDatoer(string Startpunkt, string Endepunkt, string AvreiseTid)
        {
            List<DateTime> Datoer;
            if(AvreiseTid == null)
            {
                Datoer = await _db.Ferder.Where(f => (f.Rute.Startpunkt.Equals(Startpunkt) && (f.Rute.Endepunkt.Equals(Endepunkt)))).Select(f => f.AvreiseTid).ToListAsync();
            } else
            {
                Datoer = await _db.Ferder.Where(f => (f.Rute.Startpunkt.Equals(Startpunkt) && (f.Rute.Endepunkt.Equals(Endepunkt)) && f.AnkomstTid.CompareTo(parseDato(AvreiseTid)) > 0)).Select(f => f.AvreiseTid).ToListAsync();
            }
            Datoer.Sort();
            if (Datoer == null)
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentDatoer: Ingen ferder med Startpunkt '" + Startpunkt + "' og Endepunkt '"+ Endepunkt +"' har blitt funnet i databasen.");
                return Datoer;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentDatoer: Vellykket. Ferd med Startpunkt '" + Startpunkt + "' og Endepunkt '" + Endepunkt + "' har blitt funnet i databasen.");
            return Datoer;
        }

        public async Task<DateTime> HentAnkomstTid(string AvreiseISOString)
        {
            DateTime AnkomstTid = await _db.Ferder.Where(f => f.AvreiseTid.Equals(parseDato(AvreiseISOString))).Select(f => f.AnkomstTid).FirstOrDefaultAsync();
            return AnkomstTid;
        }

        private DateTime parseDato(string dato_tid)
        {
            DateTime dato = DateTime.Parse(dato_tid, null, System.Globalization.DateTimeStyles.AssumeLocal);
            return dato; 
        }
    }
}
﻿using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Regnbuelinja.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        public static byte[] LagEnHash(string passord, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                password: passord,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 1000,
                numBytesRequested: 32);
        }

        public static byte[] LagEtSalt()
        {
            var csp = new RNGCryptoServiceProvider();
            var salt = new byte[24];
            csp.GetBytes(salt);
            return salt;
        }

        public async Task<bool> LagreRute(Ruter rute)
        {
            try
            {
                Rute lagretRute = new Rute()
                {
                    Startpunkt = rute.Avreisehavn,
                    Endepunkt = rute.Ankomsthavn,
                    Pris = Convert.ToDouble(rute.Pris)
                };

                _db.Ruter.Add(lagretRute);
                await _db.SaveChangesAsync();
                _log.LogInformation("BestillingRepository.cs: LagreRute: Rute lagret vellykket");
                return true;
            } catch
            {
                _log.LogInformation("BestillingRepository.cs: LagreRute: Feil i databasen. Rute ikke lagret");
                return false;
            }
            
        }

        public async Task<List<Rute>> HentAlleRuter()
        {
            try
            {
                List<Rute> alleRutene = await _db.Ruter.ToListAsync();
                if (!alleRutene.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleRuter: Ingen ruter i databasen");
                    return alleRutene;
                }
                else
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleRuter: Ruter hentet");
                    return alleRutene;
                }
            } catch
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleRuter: Databasefeil. Ruter ikke hentet.");
                return null;
            }
            
        }

        public async Task<int> LagreBruker(Bruker bruker)
        {
            try
            {
                var nyBruker = new Brukere
                {
                    Brukernavn = bruker.Brukernavn
                };
                
                string passord = bruker.Passord;
                byte[] salt = LagEtSalt();
                byte[] hash = LagEnHash(passord, salt);

                nyBruker.Passord = hash;
                nyBruker.Salt = salt;

                _db.Brukere.Add(nyBruker);
                await _db.SaveChangesAsync();
                _log.LogInformation("BestillingRepository.cs: LagreBruker: Ny bruker generert");
                return nyBruker.Id;
            } catch(Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: LagreBruker: Bruker ikke lagret. Databasefeil: " + e);
                return 0;
            }
        }

        public async Task<bool> LoggInn(Bruker bruker)
        {
            Brukere brukerIDB = await _db.Brukere.FirstOrDefaultAsync(b => b.Brukernavn.Equals(bruker.Brukernavn));
            if (brukerIDB == default(Brukere))
            {
                _log.LogInformation("BestillingRepository.cs: LoggInn: Ingen bruker funnet i database med brukernavn " + bruker.Brukernavn);
                return false;
            }
            else
            {
                byte[] hash = LagEnHash(bruker.Passord, brukerIDB.Salt);
                bool OK = hash.SequenceEqual(brukerIDB.Passord);
                if (OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public async Task<List<string>> HentAvgangshavner()
        {
            List<string> havner = await _db.Ruter.Select(r => r.Startpunkt).Distinct().ToListAsync();
            if (havner.Any())
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentAvgangshavner: Vellykket. Avgangshavnene ble returnert fra databasen.");
                return havner;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentAvgangshavner: Avgangshavnene ble ikke returnert fra databasen.");
            return null;
        }
        public async Task<List<string>> HentAnkomsthavner(string avgangsHavn)
        {
            List<string> havner = await _db.Ruter.Where(r => r.Startpunkt.Equals(avgangsHavn)).Select(r => r.Endepunkt).ToListAsync();
            if (havner.Any())
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentAnkomsthavner: Vellykket. Ankomsthavnene ble returnert fra databasen.");
                return havner;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentAnkomsthavner: Ankomstshavnene ble ikke returnert fra databasen.");
            return null;
        }

        //Brukes ikke(?) Slette(?)
        public async Task<List<Rute>> HentRuter(string nyttStartPunkt)
        {
            List<Rute> ruter = await _db.Ruter.Where(r => r.Startpunkt.Equals(nyttStartPunkt)).ToListAsync();
            if (!ruter.Any())
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentRuter: Ingen ruter ble returnert fra databasen.");
                return null;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentRuter: Vellykket. Rutene ble returnert fra databasen.");
            return ruter;
        }

        //Brukes ikke(?) Slette(?)
        public async Task<List<Ferd>> HentFerder(int ruteId)
        {
            List<Ferd> ferder = await _db.Ferder.Where(f => f.Rute.RId == ruteId).ToListAsync();
            if (!ferder.Any())
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentFerder: Ingen ferder ble returnert fra databasen.");
                return null;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentFerder: Vellykket. Ferdene ble returnert fra databasen.");
            return ferder;
        }

        public async Task<string> LagreBestilling(Bestilling nyBestilling)
        {
            double totalPris = 0.00;
            List<Billett> billettListe = new List<Billett>();

            DateTime AvreiseTid = parseDatoLocal(nyBestilling.AvreiseTid);
            
            Ferd ferd = await _db.Ferder.FirstOrDefaultAsync(f => f.AvreiseTid.Date.Equals(utenTimer(AvreiseTid.Date)) &&
                f.Rute.Startpunkt.Equals(nyBestilling.Startpunkt) && f.Rute.Endepunkt.Equals(nyBestilling.Endepunkt));

            Ferd ferdRetur;
            // Hvis HjemreiseTid-parameteren er definert, blir det antatt at bestillingen er en tur/retur bestilling.
            if (nyBestilling.HjemreiseTid != null)
            {
                DateTime HjemreiseTid = parseDatoLocal(nyBestilling.HjemreiseTid);
                ferdRetur = await _db.Ferder.FirstOrDefaultAsync(f => f.AvreiseTid.Date.Equals(utenTimer(HjemreiseTid.Date)) &&
                  f.Rute.Startpunkt.Equals(nyBestilling.Endepunkt) && f.Rute.Endepunkt.Equals(nyBestilling.Startpunkt));
                _log.LogInformation("/Controllers/BestillingRepository.cs: LagreBestilling: ferdRetur variablen har blitt definert.");
            }
            else
            {
                ferdRetur = null;
            }

            // Oppretter billetter for alle barn og voksne, tur retur med halv pris på barnebilletter og 25% rabatt på returbilletter.
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

                    // Hvis fer
                    if (ferdRetur != null)
                    {
                        Billett returBillett = new Billett()
                        {
                            Ferd = ferdRetur,
                            Voksen = true
                        };
                        billettListe.Add(returBillett);
                        totalPris += ferdRetur.Rute.Pris * 0.75;
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
                        totalPris += ferdRetur.Rute.Pris * 0.5 * 0.75;
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

        public async Task<Bestilling> HentBestilling(int id)
        {
            Bestilling bestilling = await _db.Bestillinger.Where(b => b.BeId == id).Select(b => new Bestilling
            {
                Startpunkt = b.Billetter.First().Ferd.Rute.Startpunkt,
                Endepunkt = b.Billetter.First().Ferd.Rute.Endepunkt,
                AvreiseTid = b.Billetter.First().Ferd.AvreiseTid.ToString("o"),
                HjemreiseTid = b.Billetter.FirstOrDefault(bi => bi.Ferd.FId != b.Billetter.First().Ferd.FId).Ferd.AvreiseTid.ToString("o"),
                AntallVoksne = b.Billetter.Where(bi => bi.Ferd.AvreiseTid.Equals(b.Billetter.First().Ferd.AvreiseTid) && bi.Voksen == true).Count(),
                AntallBarn = b.Billetter.Where(bi => bi.Ferd.AvreiseTid.Equals(b.Billetter.First().Ferd.AvreiseTid) && bi.Voksen == false).Count()
            }).FirstOrDefaultAsync();

            if (bestilling != default(Bestilling))
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
            if (pris == default)
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentPris: TotalPris ikke funnet for bestilling med id " + id);
                return pris;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentPris: Vellykket. Totalpris for bestilling med id " + id +" har blitt funnet i databasen.");
            return pris;
        }

        // Henter alle tilgjengelige datoer fra databasen. Hvis AvreiseTid-parameteren er definert, skal det kun returneres datoer minst 2 dager etter
        // datoen fra den angitte variabelen.
        public async Task<List<DateTime>> HentDatoer(string Startpunkt, string Endepunkt, string AvreiseTid)
        {
            List<DateTime> Datoer;
            if(AvreiseTid == null)
            {
                Datoer = await _db.Ferder.Where(f => (f.Rute.Startpunkt.Equals(Startpunkt) && (f.Rute.Endepunkt.Equals(Endepunkt)))).Select(f => f.AvreiseTid).ToListAsync();
            } else
            {
                DateTime AnkomstTid = await _db.Ferder.Where(f => f.Rute.Startpunkt.Equals(Startpunkt) && f.Rute.Endepunkt.Equals(Endepunkt) && f.AvreiseTid.Date.Equals(parseDatoLocal(AvreiseTid))).Select(f => f.AnkomstTid).FirstOrDefaultAsync();
                if(AnkomstTid == default)
                {
                    _log.LogInformation("/Controllers/BestillingRepository.cs: HentDatoer: Ingen ferder med startpunkt " + Startpunkt + " og endepunkt " + Endepunkt + " med avreisetid " + AvreiseTid + " funnet i databasen.");
                    return null;
                }
                Datoer = await _db.Ferder.Where(f => (f.Rute.Startpunkt.Equals(Startpunkt) && (f.Rute.Endepunkt.Equals(Endepunkt)) && f.AvreiseTid.CompareTo(AnkomstTid) > 0)).Select(f => f.AvreiseTid).ToListAsync();
            }
            
            Datoer.Sort();
            if (!Datoer.Any() || Datoer == null)
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentDatoer: Ingen ferder med Startpunkt '" + Startpunkt + "' og Endepunkt '"+ Endepunkt +"' med returdato etter avreisetid " + AvreiseTid + " har blitt funnet i databasen.");
                return null;
            }
            // endrer time tallet til 0 får å unngå problemer med Bootstrap sin Javascript kalender
            for (int i = 0; i < Datoer.Count; i++){
                Datoer[i] = utenTimer(Datoer[i]);
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentDatoer: Vellykket. Ferd med Startpunkt '" + Startpunkt + "' og Endepunkt '" + Endepunkt + "' har blitt funnet i databasen.");
            return Datoer;
        }

        public async Task<string> HentAnkomstTid(int id, string Startpunkt)
        {
            DateTime AnkomstTid = await _db.Bestillinger.Where(b => b.BeId == id).SelectMany(b => b.Billetter).Where(bi => bi.Ferd.Rute.Startpunkt.Equals(Startpunkt)).Select(bi => bi.Ferd.AnkomstTid).FirstOrDefaultAsync();
            if(AnkomstTid == default)
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentAnkomstTid: Ingen ankomsttid har blitt funnet i databasen for bestilling med id " + id +" og avreisehavn " + Startpunkt + ".");
                return null;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentAnkomstTid: Vellykket. Ankomsttid " + AnkomstTid + " har blitt funnet i databasen for bestilling med id " + id + " og avreisehavn " + Startpunkt + ".");
            return AnkomstTid.ToString("o");
        }

        public async Task<string> HentBaat(int id, string Startpunkt)
        {
            string Baatnavn = await _db.Bestillinger.Where(b => b.BeId == id).SelectMany(b => b.Billetter).Where(bi => bi.Ferd.Rute.Startpunkt.Equals(Startpunkt)).Select(bi => bi.Ferd.Baat.Navn).FirstOrDefaultAsync();
            if (Baatnavn == default)
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentBaat: Enten ingen billett med startpunkt " + Startpunkt + " eller ingen bestilling med ID " + id + " har blitt funnet i databasen");
                return null;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentBaat: Vellykket.");
            return Baatnavn;
        }

        public async Task<double> HentStrekningsPris(int id, string Startpunkt)
        {
            double strekningsPris = 0.0;
            List<Billett> billetter = await _db.Bestillinger.Where(b => b.BeId == id).SelectMany(b => b.Billetter).Where(bi => bi.Ferd.Rute.Startpunkt.Equals(Startpunkt)).ToListAsync();
            
            if (!billetter.Any())
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentStrekningsPris: Enten ingen billetter med startpunkt " + Startpunkt + " eller ingen bestilling med ID " + id + " har blitt funnet i databasen.");
                return 0.00;
            } else
            {
                int antBarn = billetter.Count(b => !b.Voksen);
                int antVoksne = billetter.Count(b => b.Voksen);
                double prisPerBillett = billetter.First().Ferd.Rute.Pris;
                strekningsPris += (antBarn * 0.5 * prisPerBillett) + (antVoksne * prisPerBillett);
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentStrekningsPris: Vellykket. Strekningspris " + strekningsPris + " funnet for " + antBarn + " barn og " + antVoksne + " voksne fra avreisehavn " + Startpunkt + " for bestilling med id " + id);
                return strekningsPris;
            }
            
        }

        // En lokal metode for å konvertere dato strenger til DateTime objekter
        private DateTime parseDatoLocal(string dato_tid)
        {
            DateTime dato = DateTime.Parse(dato_tid, null, System.Globalization.DateTimeStyles.AssumeLocal);
            return dato; 
        }

        private DateTime utenTimer(DateTime dato)
        {
            return new DateTime(dato.Year, dato.Month, dato.Day, 0, 0 ,0); 
        }

    }
}
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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

        public async Task<bool> LagreRute(Ruter rute)
        {
            try
            {
                Rute lagretRute = new Rute()
                {
                    Startpunkt = rute.Startpunkt,
                    Endepunkt = rute.Endepunkt,
                    Pris = rute.Pris
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

        public async Task<bool> EndreRute(Ruter endreRute)
        {
            try
            {
                Rute somSkalEndres = await _db.Ruter.FindAsync(endreRute.Id);
                if (!(endreRute == null))
                {
                    List<Billett> AlleBilletter = await _db.Billetter.Where(b => b.Ferd.Rute.Id == somSkalEndres.Id).ToListAsync();
                    if (AlleBilletter.Any())
                    {
                        _log.LogInformation("BestillingRepository.cs: EndreRute: Rute med i en bestillt ferd. Ikke endret");
                        return false;
                    }

                    somSkalEndres.Startpunkt = endreRute.Startpunkt;
                    somSkalEndres.Endepunkt = endreRute.Endepunkt;
                    somSkalEndres.Pris = endreRute.Pris;

                    await _db.SaveChangesAsync();
                    _log.LogInformation("BestillingRepository.cs: EndreRute: Vellykket! Rute endret");
                    return true;

                }
                else
                {
                    _log.LogInformation("BestillingRepository.cs: EndreRute: Rute finnes ikke i databasen");
                    return false;
                }
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: EndreRute: Feil i databasen. Rute ikke endret. " + e);
                return false;
            }

        }

        //Hvis rute er med i en bestilling kan ikke ruta slettes. Dersom ruta likevel kan slettes, slettes også alle ferdene ruta er med på
        public async Task<bool> SlettRute(int id)
        {
            try
            {
                Rute fjerneRute = await _db.Ruter.FindAsync(id);
                if (!(fjerneRute == null))
                {
                    List<Billett> AlleRelaterteBilletter = await _db.Billetter.Where(b => b.Ferd.Rute.Id == fjerneRute.Id).ToListAsync();
                    if (AlleRelaterteBilletter.Any())
                    {
                        _log.LogInformation("BestillingRepository.cs: SlettRute: Rute med i en bestillt ferd. Ikke slettet");
                        return false;
                    }
                    List<Ferd> AlleRelaterteFerder = await _db.Ferder.Where(f => f.Rute.Id == id).ToListAsync();
                    if (AlleRelaterteFerder.Any())
                    {
                        foreach (Ferd ferd in AlleRelaterteFerder)
                        {
                            _log.LogInformation("BestillingRepository.cs: SlettRute: Ferd med id " + ferd.Id + " slettet");
                            _db.Ferder.Remove(ferd);
                        }
                    }

                    _db.Ruter.Remove(fjerneRute);
                    await _db.SaveChangesAsync();
                    _log.LogInformation("BestillingRepository.cs: SlettRute: Vellykket! Rute slettet");
                    return true;

                } else
                {
                    _log.LogInformation("BestillingRepository.cs: SlettRute: Rute finnes ikke i databasen");
                    return false;
                }
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: SlettRute: Feil i databasen. Rute ikke slettet. " + e);
                return false;
            }
        }

        public async Task<Rute> HentEnRute(int id)
        {
            try
            {
                Rute hentetRute = await _db.Ruter.FirstOrDefaultAsync(r => r.Id == id);
                if (hentetRute == default)
                {
                    _log.LogInformation("BestillingRepository.cs: HentEnRute: Rute ikke funnet i databasen");
                }
                return hentetRute;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepisotory.cs: HentEnRute: Feil i databasen på serveren: " + e);
                return null;
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
                }
                _log.LogInformation("BestillingRepository.cs: HentAlleRuter: Vellykket! Ruter hentet");
                return alleRutene;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleRuter: Databasefeil: " + e + ". Ruter ikke hentet");
                return null;
            }

        }

        public async Task<bool> LagreBåt(Baater båt) {
            try
            {
                Baat nyBaat = new Baat
                {
                    Navn = båt.Navn
                };

                _db.Baater.Add(nyBaat);
                _log.LogInformation("BestillingRepository.cs: LagreBåt: Vellykket! Båt lagret i databasen");
                await _db.SaveChangesAsync();
                return true;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: LagreBåt: Feil i databasen: " + e + ". Båt ikke lagret");
                return false;
            }

        }

        //Båt i bestilling kan endres, men ikke slettes - må oppdateres hvis vi implementerer kapasitet.
        public async Task<bool> EndreBåt(Baater båt)
        {
            try
            {
                Baat somSkalEndres = await _db.Baater.FirstOrDefaultAsync(b => b.Id == båt.Id);
                if (somSkalEndres != default)
                {
                    somSkalEndres.Navn = båt.Navn;
                    await _db.SaveChangesAsync();
                    _log.LogInformation("BestillingRepository.cs: EndreBåt: Vellykket! Båt endret");
                    return true;
                }
                _log.LogInformation("BestillingRepository.cs: EndreBåt: Fant ikke båten i databasen");
                return false;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: EndreBåt: Feil i databasen: " + e + ". Båt ikke endret");
                return false;
            }

        }


        // En båt kan ikke slettes dersom båten er med på en ferd i en bestilling. Hvis den likevel kan slettes, slettes også alle ferdene.
        public async Task<bool> SlettBåt(int id)
        {
            try
            {
                Baat somSkalSlettes = await _db.Baater.FindAsync(id);
                if (somSkalSlettes != null)
                {
                    List<Billett> AlleRelaterteBilletter = await _db.Billetter.Where(b => b.Ferd.Baat.Id == id).ToListAsync();
                    if (AlleRelaterteBilletter.Any())
                    {
                        _log.LogInformation("BestillingRepository.cs: SlettBåt: Båt med i en bestillt ferd. Ikke slettet");
                        return false;
                    }
                    List<Ferd> AlleRelaterteFerder = await _db.Ferder.Where(f => f.Baat.Id == id).ToListAsync();
                    if (AlleRelaterteFerder.Any())
                    {
                        foreach (Ferd ferd in AlleRelaterteFerder)
                        {
                            _log.LogInformation("BestillingRepository.cs: SlettBåt: Ferd med id " + ferd.Id + " slettet");
                            _db.Ferder.Remove(ferd);
                        }
                    }

                    _db.Baater.Remove(somSkalSlettes);
                    _log.LogInformation("BestillingRepository.cs: SlettBåt: Vellykket! Båt og relaterte ferder slettet");
                    await _db.SaveChangesAsync();
                    return true;
                }
                _log.LogInformation("BestillingRepository.cs: SlettBåt: Fant ikke båten i databasen");
                return false;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: SlettBåt: Feil i databasen: " + e + ". Båt ikke slettet");
                return false;
            }
        }

        public async Task<List<Baat>> HentAlleBåter()
        {
            try
            {
                List<Baat> alleBåtene = await _db.Baater.ToListAsync();
                _log.LogInformation("BestillingRepository.cs: HentAlleBåter: Vellykket! Båter hentet");
                return alleBåtene;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleBåter: Databasefeil: " + e + ". Båter ikke hentet");
                return null;
            }
        }

        public async Task<Baat> HentEnBåt(int id)
        {
            try
            {
                Baat hentetBåt = await _db.Baater.FirstOrDefaultAsync(b => b.Id == id);
                if (hentetBåt == default)
                {
                    _log.LogInformation("BestillingRepository.cs: HentEnBåt: Ingen båt funnet i databasen med gitt id");
                }
                _log.LogInformation("BestillingRepository.cs: HentEnBåt: Vellykket! Båt hentet");
                return hentetBåt;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepisotory.cs: HentEnBaat: Feil i databasen på serveren: " + e);
                return null;
            }
        }


        // Fiks test at ankomstTid er etter avreiseTid
        public async Task<bool> LagreFerd(Ferder ferdSomLagres)
        {
            try
            {
                Rute rute = await _db.Ruter.FirstOrDefaultAsync(r => r.Id == ferdSomLagres.RId);
                Baat båt = await _db.Baater.FirstOrDefaultAsync(b => b.Id == ferdSomLagres.BId);
                if (rute == default || båt == default)
                {
                    _log.LogInformation("BestillingRepository.cs: Ferd ikke lagret. Rute eller båt ikke i databasen");
                    return false;
                }
                DateTime avreiseTid = ParseDatoLocal(ferdSomLagres.AvreiseTid);
                DateTime ankomstTid = ParseDatoLocal(ferdSomLagres.AnkomstTid);
                Ferd nyFerd = new Ferd()
                {
                    Rute = rute,
                    Baat = båt,
                    AvreiseTid = avreiseTid,
                    AnkomstTid = ankomstTid
                };

                _db.Ferder.Add(nyFerd);
                await _db.SaveChangesAsync();
                _log.LogInformation("BestillingRepository.cs: Vellykket! Ferd lagret i databasen");
                return true;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: Databasefeil: " + e + ". Ferd ikke lagret.");
                return false;
            }
        }

        public async Task<List<Ferder>> HentAlleFerder()
        {
            try
            {
                List<Ferder> alleFerdene = await _db.Ferder.Select(f => new Ferder() {
                    FId = f.Id,
                    BId = f.Baat.Id,
                    RId = f.Rute.Id,
                    AvreiseTid = f.AvreiseTid.ToString("o"),
                    AnkomstTid = f.AnkomstTid.ToString("o")
                }).ToListAsync();
                if (!alleFerdene.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleFerder: Ingen ferder i databasen");
                }
                _log.LogInformation("BestillingRepository.cs: HentAlleFerder: Vellykket! Ferder hentet");
                return alleFerdene;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleFerder: Databasefeil: " + e + ". Ferder ikke hentet");
                return null;
            }
        }

        public async Task<Ferder> HentEnFerd(int id)
        {
            try
            {
                Ferder hentetFerd = await _db.Ferder.Where(f => f.Id == id).Select(f => new Ferder() {
                    FId = f.Id,
                    BId = f.Baat.Id,
                    RId = f.Rute.Id,
                    AvreiseTid = f.AvreiseTid.ToString("o"),
                    AnkomstTid = f.AnkomstTid.ToString("o")
                }).FirstOrDefaultAsync();
                if (hentetFerd == default)
                {
                    _log.LogInformation("BestillingRepository.cs: HentEnFerd: Ingen ferd funnet i databasen med gitt id");
                }
                _log.LogInformation("BestillingRepository.cs: HentEnFerd: Vellykket! Ferd hentet");
                return hentetFerd;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentEnFerd: Feil i databasen på serveren: " + e);
                return null;
            }
        }

        // En bestillt ferd kan ikke endres
        public async Task<bool> EndreFerd(Ferder ferd)
        {
            try
            {
                Ferd ferdSomEndres = await _db.Ferder.FirstOrDefaultAsync(f => f.Id == ferd.FId);
                if (!(ferdSomEndres == default))
                {
                    List<Billett> AlleBilletter = await _db.Billetter.Where(b => b.Ferd.Id == ferdSomEndres.Id).ToListAsync();
                    if (AlleBilletter.Any())
                    {
                        _log.LogInformation("BestillingRepository.cs: EndreFerd: Ferd med i bestilling(er). Ikke endret");
                        return false;
                    }
                    Rute nyRute = await _db.Ruter.FirstOrDefaultAsync(r => r.Id == ferd.RId);
                    Baat nyBåt = await _db.Baater.FirstOrDefaultAsync(b => b.Id == ferd.BId);
                    if (nyRute == default || nyBåt == default)
                    {
                        _log.LogInformation("BestillingRepository.cs: EndreFerd: Rute eller båt ikke funnet i database. Kan ikke endre");
                        return false;
                    }
                    ferdSomEndres.Rute = nyRute;
                    ferdSomEndres.Baat = nyBåt;
                    ferdSomEndres.AvreiseTid = ParseDatoLocal(ferd.AvreiseTid);
                    ferdSomEndres.AnkomstTid = ParseDatoLocal(ferd.AnkomstTid);
                    await _db.SaveChangesAsync();
                    _log.LogInformation("BestillingRepository.cs: EndreFerd: Vellykket! Ferd endret");
                    return true;
                }
                _log.LogInformation("BestillingRepository.cs: EndreFerd: Ferd ikke funnet med gitt id");
                return false;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: EndreFerd: Databasefeil: " + e + ". Ferd ikke endret");
                return false;
            }
        }

        // Kun ferder som ikke er bestillt kan slettes fra databasen.
        public async Task<bool> SlettFerd(int id)
        {
            try
            {
                Ferd fjerneFerd = await _db.Ferder.FirstOrDefaultAsync(f => f.Id == id);
                if (!(fjerneFerd == default))
                {
                    List<Billett> AlleRelaterteBilletter = await _db.Billetter.Where(b => b.Ferd.Id == id).ToListAsync();
                    if (AlleRelaterteBilletter.Any())
                    {
                        _log.LogInformation("BestillingRepository.cs: SlettFerd: Ferd i bestilling(er). Ikke slettet");
                        return false;
                    }

                    _db.Ferder.Remove(fjerneFerd);
                    await _db.SaveChangesAsync();
                    _log.LogInformation("BestillingRepository.cs: SlettFerd: Vellykket! Ferd slettet");
                    return true;
                }
                else
                {
                    _log.LogInformation("BestillingRepository.cs: SlettFerd: Ferd finnes ikke i databasen");
                    return false;
                }
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: SlettFerd: Feil i databasen. Ferd ikke slettet. " + e);
                return false;
            }
        }

        public async Task<List<Bestilling>> HentAlleBestillinger()
        {
            try
            {
                List<Bestillinger> alleBestillinger = await _db.Bestillinger.ToListAsync();
                List<Bestilling> hentedeBestillinger = new List<Bestilling>();
                foreach (Bestillinger enBestilling in alleBestillinger)
                {
                    double TotalPris = await BeregnTotalPris(enBestilling.Id);
                    Bestilling bestilling = new Bestilling()
                    {
                        Id = enBestilling.Id,
                        KId = enBestilling.Kunde.Id,
                        Betalt = enBestilling.Betalt,
                        Totalpris = TotalPris
                    };
                    hentedeBestillinger.Add(bestilling);
                }

                if (!hentedeBestillinger.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleBestillinger: Ingen bestillinger i databasen");
                }
                return hentedeBestillinger;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleBestillinger: Feil i databasen: " + e + ". Bestillinger ikke hentet");
                return null;
            }
        }

        public async Task<Bestilling> HentEnBestilling(int id)
        {
            try
            {
                double TotalPris = await BeregnTotalPris(id);
                Bestilling bestilling = _db.Bestillinger.Where(b => b.Id == id).Select(b => new Bestilling() {
                    Id = b.Id,
                    KId = b.Kunde.Id,
                    Betalt = b.Betalt,
                    Totalpris = TotalPris
                }).FirstOrDefault();
                if (bestilling == default)
                {
                    _log.LogInformation("BestillingRepository.cs: HentEnBestilling: Ingen bestilling med id " + id + " i databasen");
                }
                return bestilling;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentEnBestilling: Feil i databasen: " + e + ". Bestilling ikke hentet");
                return null;
            }
        }

        // En bestilling kan slettes (etter kundeønske) hvis den ikke er betalt enda OG ferden ikke har vært (med beskjed til kunde) ELLER
        // hvis ferden allerede er gjennomført (ankomsttid er tidligere enn dagens dato) OG den er betalt. Dette sikrer at man ikke slettes uoppgjorte
        // bestillinger som er gjennomført og
        // at man ikke sletter billetter og bestillinger til kunder som har betalt, men ikke har reist enda.
        // Fremtidig ville man da kunne implementere en metode som eksporterte ubetalte gjennomførte ferder til regnskapsavdelingen dersom man ønsket
        // å slette historiske ferder.

        // Billetter i bestilling slettes før bestillingen slettes.

        public async Task<bool> SlettBestilling(int id)
        {
            try
            {
                Bestillinger somSkalSlettes = await _db.Bestillinger.FindAsync(id);
                if (somSkalSlettes != null)
                {
                    Ferd ferd = somSkalSlettes.Billetter.First().Ferd;
                    Ferd returFerd = somSkalSlettes.Billetter.Where(b => b.Ferd.Id != ferd.Id).Select(b => b.Ferd).FirstOrDefault();
                    DateTime ankomstTid = returFerd == default ? ferd.AnkomstTid : returFerd.AnkomstTid;
                    if ((somSkalSlettes.Betalt && ankomstTid.CompareTo(DateTime.Now) < 0) || (!somSkalSlettes.Betalt && (ankomstTid.CompareTo(DateTime.Now) > 0)))
                    {
                        foreach (Billett billett in somSkalSlettes.Billetter)
                        {
                            _log.LogInformation("BestillingRepository.cs: SlettBestilling: Billett med id " + id + " slettet");
                            _db.Billetter.Remove(billett);
                        }
                        _log.LogInformation("BestillingRepository.cs: SlettBestilling: Vellykket. Bestilling slettet");
                        _db.Remove(somSkalSlettes);
                        await _db.SaveChangesAsync();
                        return true;
                    }
                    _log.LogInformation("BestillingRepository.cs: SlettBestilling: Bestillingen er ikke betalt og gjennomført ELLER betalt, men ikke gjennomført. Ikke slettet");
                    return false;
                }
                _log.LogInformation("BestillingRepository.cs: SlettBestilling: Fant ikke bestillingen i databasen");
                return false;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: SlettBestilling: Databasefeil: " + e);
                return false;
            }
        }


        // En ubetalt bestilling for en reise framover i tid kan endres (ved kundeønske), mens en betalt bestilling er fastsatt og kan ikke endres (faktureringsavdeling tar seg av dette,
        // vi vil holde logikken så enkel som mulig).

        public async Task<bool> EndreBestilling(Bestilling bestilling)
        {
            try
            {
                Bestillinger endreBestilling = await _db.Bestillinger.FindAsync(bestilling.Id);
                if (endreBestilling != null)
                {
                    if (!endreBestilling.Betalt)
                    {
                        foreach (Billett billett in endreBestilling.Billetter)
                        {
                            if (billett.Ferd.AnkomstTid.CompareTo(DateTime.Now) < 0)
                            {
                                _log.LogInformation("BestillingRepository.cs: EndreBestilling: Kan ikke endres da den inneholder reiser som allerede har vært");
                                return false;
                            }
                        }
                        Person kunde = await _db.KunderOgAnsatte.FirstOrDefaultAsync(k => k.Id == bestilling.KId);
                        if (kunde != default)
                        {
                            endreBestilling.Kunde = kunde;
                            endreBestilling.Betalt = bestilling.Betalt;

                            await _db.SaveChangesAsync();
                            _log.LogInformation("BestillingRepository.cs: EndreBestilling: Vellykket. Bestilling endret");
                            return true;
                        }
                        _log.LogInformation("BestillingRepository.cs: EndreBestilling: Kunde ikke funnet. Kunne ikke endre bestilling");
                        return false;
                    }
                    _log.LogInformation("BestillingRepository.cs: EndreBestilling: Bestillingen er betalt. Kan ikke endres");
                    return false;
                }
                _log.LogInformation("BestillingRepository.cs: EndreBestilling: Fant ikke bestillingen i databasen");
                return false;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: EndreBestilling: Databasefeil: " + e);
                return false;
            }
        }

        public async Task<Billetter> HentEnBillett(int id)
        {
            try
            {
                Billetter billett = await _db.Billetter.Select(b => new Billetter()
                {
                    Id = b.Id,
                    FId = b.Ferd.Id,
                    BId = b.Bestilling.Id,
                    Voksen = b.Voksen
                }).FirstOrDefaultAsync();
                if (billett == default)
                {
                    _log.LogInformation("BestillingRepository.cs: HentEnBillett: Ingen billett med id " + id + " i databasen");
                }
                return billett;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentEnBillett: Feil i databasen: " + e + ". Billett ikke hentet");
                return null;
            }
        }

        // Hvis kunde ønsker å legge til flere billetter til bestillingen sin. Krever at bestillingsId finnes,
        // og at reisen ikke har vært og er ubetalt (igjen, for enkelhetsskyld => sendes til regnskapsavdelingen)
        // Det kreves også at ferd er lik originalbilletten, eller lik en eventuell retur-reise (med passende avreisetid). Hvis man skal legge til
        // billett for en annen strekning må en ny bestilling opprettes.
        public async Task<bool> LagreBillett(Billetter billett)
        {
            try
            {
                Bestillinger bestilling = await _db.Bestillinger.FindAsync(billett.BId);
                Ferd ferd = await _db.Ferder.FindAsync(billett.FId);

                if (bestilling != null && ferd != null && !bestilling.Betalt)
                {
                    if (!bestilling.Billetter.Any())
                    {
                        Billett nyBillett = new Billett()
                        {
                            Bestilling = bestilling,
                            Ferd = ferd,
                            Voksen = billett.Voksen
                        };

                        _db.Billetter.Add(nyBillett);
                        await _db.SaveChangesAsync();
                        _log.LogInformation("BestillingRepository.cs: LagreBillett: Første billett lagret til bestilling");
                        return true;
                    }

                    else
                    {
                        Ferd IBestilling = bestilling.Billetter.First().Ferd;
                        Ferd returFerd = bestilling.Billetter.Where(b => b.Ferd != IBestilling).Select(b => b.Ferd).FirstOrDefault();

                        if(returFerd != null)
                        {
                            if(ferd.Id == IBestilling.Id || returFerd.Id == ferd.Id)
                            {
                                Billett nyBillett = new Billett()
                                {
                                    Bestilling = bestilling,
                                    Ferd = ferd,
                                    Voksen = billett.Voksen
                                };
                                _log.LogInformation("BestillingRepository.cs: LagreBillett: Billett lagt til bestilling med billetter");
                                _db.Billetter.Add(nyBillett);
                                await _db.SaveChangesAsync();
                                return true;
                            }
                            _log.LogInformation("BestillingRepository.cs: Kan ikke lagre billett som ikke er tur/retur-ferd på denne bestillingen");
                            return false;
                        } 
                        string StartpunktIBestilling = IBestilling.Rute.Startpunkt;
                        string EndepunktIBestilling = IBestilling.Rute.Endepunkt;

                        // Hvis ferden i nybillett er lik ferden til eksisterende billett eller det er en returferd (med passende avreise/ankomsttid)
                        
                        if ((ferd == IBestilling) || (EndepunktIBestilling.Equals(ferd.Rute.Startpunkt) && StartpunktIBestilling.Equals(ferd.Rute.Endepunkt) &&
                                (ferd.AvreiseTid.CompareTo(IBestilling.AnkomstTid) > 0)))
                        {

                            Billett nyBillett = new Billett()
                            {
                                Bestilling = bestilling,
                                Ferd = ferd,
                                Voksen = billett.Voksen
                            };
                            _log.LogInformation("BestillingRepository.cs: LagreBillett: Billett lagt til bestilling med billetter");
                            _db.Billetter.Add(nyBillett);
                            await _db.SaveChangesAsync();
                            return true;
                        }
                        _log.LogInformation("BestillingRepository.cs: LagreBillett: Kan ikke legge til billett for annen rute enn rute i " +
                            "bestilling eller returrute. Avreise/ankomsttid må også stemme overens");
                       return false;
                    }
                }
                _log.LogInformation("BestillingRepository.cs: LagreBillett: Bestillingen er allerede betalt eller" +
                            " billetten har reise som allerede har vært");
                return false;

            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: LagreBillett: Databasefeil: " + e);
                return false;
            }
        }

        // Kan kun endre en billett for en ubetalt ferd som er framover i tid. "Regnskapsavdelingen" vil håndtere betalte billetter (herunder refusjon/ekstra
        // betaling utfra rutepris)
        // Bestilling - fremmednøkkel endres ikke da billetten er knyttet til kunde via bestilling.
        // Endrer kun mellom voksen og barn ettersom bestillingen er knyttet til en spesifikk rute og ferd - ny bestilling og billett hvis kunden skal til en ny destinasjon

        public async Task<bool> EndreBillett(Billetter billett)
        {
            try
            {
                Billett endreBillett = await _db.Billetter.FindAsync(billett.Id);
                if (endreBillett != null)
                {
                    Bestillinger bestilling = endreBillett.Bestilling;
                    if (!bestilling.Betalt && (endreBillett.Ferd.AnkomstTid.CompareTo(DateTime.Now) > 0))
                    {
                        if (endreBillett.Voksen) endreBillett.Voksen = false;
                        else endreBillett.Voksen = true;

                        _log.LogInformation("BestillingRepository.cs: EndreBillett: Vellykket! Billetten er endret");
                        await _db.SaveChangesAsync();
                        return true;

                    }
                    _log.LogInformation("BestillingRepository.cs: EndreBillett: Bestillingen er betalt eller reisa er gjennomført. Kan ikke endre billett");
                    return false;
                }
                _log.LogInformation("BestillingRepository.cs: EndreBillett: Fant ikke billett i databasen");
                return false;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: EndreBillett: Databasefeil: " + e);
                return false;
            }
        }


        //Kan bare slette billetter for ubetalte reiser framover i tid eller betalte reiser som har vært.
        public async Task<bool> SlettBillett(int id)
        {
            try
            {
                Billett somSkalSlettes = await _db.Billetter.FindAsync(id);
                if (somSkalSlettes != null)
                {
                    bool reiseFramoverITid = somSkalSlettes.Ferd.AnkomstTid.CompareTo(DateTime.Now) > 0;
                    if ((somSkalSlettes.Bestilling.Betalt && !reiseFramoverITid) || (!somSkalSlettes.Bestilling.Betalt && reiseFramoverITid))
                    {
                        _log.LogInformation("BestillingRepository.cs: SlettBestilling: Vellykket. Bestilling slettet");
                        _db.Remove(somSkalSlettes);
                        await _db.SaveChangesAsync();
                        return true;
                    }
                    _log.LogInformation("BestillingRepository.cs: SlettBestilling: Bestillingen er ikke betalt enda. Kan ikke slettes");
                    return false;
                }
                _log.LogInformation("BestillingRepository.cs: SlettBestilling: Fant ikke bestillingen i databasen");
                return false;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: SlettBestilling: Databasefeil: " + e);
                return false;
            }
        }

        public async Task<List<Billetter>> HentAlleBilletter()
        {
            try
            {
                List<Billetter> alleBilletter = await _db.Billetter.Select(b => new Billetter()
                {
                    Id = b.Id,
                    FId = b.Ferd.Id,
                    BId = b.Bestilling.Id,
                    Voksen = b.Voksen
                }).ToListAsync();
                if (!alleBilletter.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleBilletter: Ingen billetter i databasen");
                }
                return alleBilletter;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleBilletter: Feil i databasen: " + e + ". Billetter ikke hentet");
                return null;
            }
        }

        public async Task<List<Billetter>> HentBilletterForBestilling(int id)
        {
            try
            {
                List<Billetter> alleBilletter = await _db.Billetter.Where(b => b.Bestilling.Id == id).Select(b => new Billetter()
                {
                    Id = b.Id,
                    FId = b.Ferd.Id,
                    BId = b.Bestilling.Id,
                    Voksen = b.Voksen
                }).ToListAsync();
                if (!alleBilletter.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForBestilling: Bestilling ikke funnet eller ingen billetter i bestilling");
                }
                return alleBilletter;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForBestilling: Feil i databasen: " + e + ". Billetter ikke hentet");
                return null;
            }
        }

        public async Task<List<Billetter>> HentBilletterForFerd(int id)
        {
            try
            {
                List<Billetter> alleBilletter = await _db.Billetter.Where(b => b.Ferd.Id == id).Select(b => new Billetter()
                {
                    Id = b.Id,
                    BId = b.Bestilling.Id,
                    FId = b.Ferd.Id,
                    Voksen = b.Voksen
                }).ToListAsync();

                if (!alleBilletter.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForFerd: Ferd ikke funnet eller ingen billetter for ferd");
                }
                _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForFerd: Vellykket! Billetter hentet");
                return alleBilletter;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForFerd: Feil i databasen: " + e + ". Billetter ikke hentet");
                return null;
            }
        }

        public async Task<List<Billetter>> HentBilletterForRute(int id)
        {
            try
            {
                List<Billetter> alleBilletter = await _db.Billetter.Where(b => b.Ferd.Rute.Id == id).Select(b => new Billetter()
                {
                    Id = b.Id,
                    BId = b.Bestilling.Id,
                    FId = b.Ferd.Id,
                    Voksen = b.Voksen
                }).ToListAsync();

                if (!alleBilletter.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForRute: Rute ikke funnet eller ingen billetter for rute");
                }
                _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForRute: Vellykket! Billetter hentet");
                return alleBilletter;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForRute: Feil i databasen: " + e + ". Billetter ikke hentet");
                return null;
            }
        }

        public async Task<List<Billetter>> HentBilletterForBåt(int id)
        {
            try
            {
                List<Billetter> alleBilletter = await _db.Billetter.Where(b => b.Ferd.Baat.Id == id).Select(b => new Billetter()
                {
                    Id = b.Id,
                    BId = b.Bestilling.Id,
                    FId = b.Ferd.Id,
                    Voksen = b.Voksen
                }).ToListAsync();

                if (!alleBilletter.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentBilletterForBåt: Båt ikke funnet eller ingen billetter for båt");
                }
                _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForBåt: Vellykket! Billetter hentet");
                return alleBilletter;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentBilletterForBåt: Feil i databasen: " + e + ". Billetter ikke hentet");
                return null;
            }
        }

        // Henter gyldige ferder som kan velges for en ny billett i en eksisterende bestilling.
        // Hvis ingen billetter ligger inne hentes alle ferder
        // Hvis en ferd og en returferd ligger i bestillingen returneres disse
        // Hvis kun en ferd er bestillt returneres ferden samt alle returferder for samme rute.
        public async Task<List<FerdRute>> HentFerdRuterForBestilling(int id)
        {
            try
            {
                Bestillinger bestilling = await _db.Bestillinger.FindAsync(id);
                if (bestilling == default)
                {
                    _log.LogInformation("BestillingRepository.cs: HentFerdRuterForBestilling: Bestilling ikke funnet i databasen");
                    return null;
                }
                List<FerdRute> gyldigeFerder;
                if (!bestilling.Billetter.Any())
                {
                    gyldigeFerder = await _db.Ferder.Select(f => new FerdRute()
                    {
                        FId = f.Id,
                        AvreiseTid = f.AvreiseTid.ToString("o"),
                        AnkomstTid = f.AnkomstTid.ToString("o"),
                        Strekning = (f.Rute.Startpunkt + " - " + f.Rute.Endepunkt)
                    }).ToListAsync();
                } else
                {
                    gyldigeFerder = new List<FerdRute>();

                    Ferd ferd = bestilling.Billetter.First().Ferd;
                    FerdRute ferdSomHentes = new FerdRute() {
                        FId = ferd.Id,
                        AnkomstTid = ferd.AnkomstTid.ToString("o"),
                        AvreiseTid = ferd.AvreiseTid.ToString("o"),
                        Strekning = (ferd.Rute.Startpunkt + " - " + ferd.Rute.Endepunkt)
                    };

                    gyldigeFerder.Add(ferdSomHentes);

                    Ferd returFerd = bestilling.Billetter.Where(b => b.Ferd.Id != ferd.Id).Select(b => b.Ferd).FirstOrDefault();
                    if (returFerd != default)
                    {
                        FerdRute returFerdRute = new FerdRute()
                        {
                            FId = ferd.Id,
                            AvreiseTid = returFerd.AvreiseTid.ToString("o"),
                            AnkomstTid = returFerd.AnkomstTid.ToString("o"),
                            Strekning = (returFerd.Rute.Startpunkt + " - " + ferd.Rute.Endepunkt)
                        };
                        gyldigeFerder.Add(returFerdRute);
                        _log.LogInformation("BestillingRepository.cs: HentFerdRuterForBestilling: Vellykket! Ferd og returferd returnert");
                    } else
                    {
                        string startpunkt = ferd.Rute.Startpunkt;
                        string endepunkt = ferd.Rute.Endepunkt;
                        DateTime AnkomstTid = ferd.AnkomstTid;
                        gyldigeFerder = await _db.Ferder.Where(f => (((f.Rute.Startpunkt.Equals(endepunkt)) && (f.Rute.Endepunkt.Equals(startpunkt)) && f.AvreiseTid.CompareTo(AnkomstTid) > 0))).Select(f => new FerdRute()
                        {
                            FId = f.Id,
                            AvreiseTid = f.AvreiseTid.ToString("o"),
                            AnkomstTid = f.AnkomstTid.ToString("o"),
                            Strekning = (f.Rute.Startpunkt + " - " + f.Rute.Endepunkt)
                        }).ToListAsync();
                        gyldigeFerder.Add(ferdSomHentes);
                    }
                }
                if (!gyldigeFerder.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentFerdRuterForBestilling: Ingen gyldige ferder funnet");
                }
                _log.LogInformation("BestillingRepository.cs: HentFerdRuterForBestilling: Gyldige ferder returnert");
                return gyldigeFerder;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentFerdRuterForBestilling: Databasefeil: " + e);
                return null;
            }
        }

        public async Task<List<Bestilling>> HentBestillingerForKunde(int id)
        {
            try
            {
                List<Bestillinger> alleBestillinger = await _db.Bestillinger.Where(b => b.Kunde.Id == id).ToListAsync();
                List<Bestilling> hentedeBestillinger = new List<Bestilling>();
                foreach (Bestillinger enBestilling in alleBestillinger)
                {
                    double TotalPris = await BeregnTotalPris(enBestilling.Id);
                    Bestilling bestilling = new Bestilling()
                    {
                        Id = enBestilling.Id,
                        KId = enBestilling.Kunde.Id,
                        Totalpris = TotalPris,
                        Betalt = enBestilling.Betalt
                    };
                    hentedeBestillinger.Add(bestilling);
                }

                if (!hentedeBestillinger.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentBestillingerForKunde: Kunde ikke funnet eller kunden har ikke bestillt neon reise enda");
                }
                _log.LogInformation("BestillingRepository.cs: HentBestillingerForKunde: Vellykket! Bestillinger hentet");
                return hentedeBestillinger;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentBestillingerForKunde: Feil i databasen: " + e + ". Bestillinger ikke hentet");
                return null;
            }
        }

        public async Task<bool> EndreBruker(Bruker bruker, string username)
        {
            try
            {
                Brukere somSkalEndres = await _db.Brukere.FirstOrDefaultAsync(b => b.Brukernavn.Equals(bruker.Brukernavn));
                if(somSkalEndres != default && somSkalEndres.Brukernavn == username)
                {
                    string passord = bruker.Passord;
                    byte[] salt = LagEtSalt();
                    byte[] hash = LagEnHash(passord, salt);

                    somSkalEndres.Passord = hash;
                    somSkalEndres.Salt = salt;

                    await _db.SaveChangesAsync();
                    _log.LogInformation("BestillingRepository.cs: EndreBruker: Bruker endret");
                    return true;
                }
                _log.LogInformation("BestillingRepository.cs: EndreBruker: Fant ikke bruker i databasen.");
                return false;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: EndreBruker: Bruker ikke endret. Databasefeil: " + e);
                return false;
            }
        }

        public async Task<bool> LagreBruker(Bruker bruker)
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
                return true;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: LagreBruker: Bruker ikke lagret. Databasefeil: " + e);
                return false;
            }
        }

        // I utgangspunktet er det kunder selv som skal registrere seg, men har lagt inn funksjonaliteten likevel
        // (I tilfelle admin også vil bestille billetter)
        public async Task<int> LagreKunde(Personer kunde)
        {
            try
            {
                Person NyKunde = new Person()
                {
                    Fornavn = kunde.Fornavn,
                    Etternavn = kunde.Etternavn,
                    Epost = kunde.Epost,
                    Telefonnr = kunde.Telefonnr
                };

                _db.KunderOgAnsatte.Add(NyKunde);
                await _db.SaveChangesAsync();
                int kid = await _db.KunderOgAnsatte.Where(k => k.Fornavn.Equals(NyKunde.Fornavn) && k.Etternavn.Equals(NyKunde.Etternavn)).Select(k => k.Id).FirstOrDefaultAsync();
                _log.LogInformation("BestillingRepository.cs: LagreKunde: Kunde lagret");
                return kid;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: LagreKunde: Feil i databasen: " + e + ". Kunde ikke lagret");
                return 0;
            }
        }

        public async Task<bool> LeggKundeTilBestilling(int BId, int KId)
        {
            try
            {
                Bestillinger bestilling = await _db.Bestillinger.FindAsync(BId);
                Person kunde = await _db.KunderOgAnsatte.FindAsync(KId);
                if (kunde != null && bestilling != null)
                {
                    bestilling.Kunde = kunde;
                    kunde.Bestillinger.Add(bestilling);
                    await _db.SaveChangesAsync();
                    return true;
                }
                _log.LogInformation("BestillingRepository.cs: LeggKundeTilBestilling: Fant ikke kunden eller bestillingen");
                return false;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: LeggKundeTilBestilling: Databasefeil: " + e);
                return false;
            }
        }

        public async Task<Personer> HentEnKunde(int id)
        {
            try
            {
                Personer hentetKunde = await _db.KunderOgAnsatte.Where(k => k.Id == id && k.Admin == false).Select(k => new Personer()
                {
                    Id = k.Id,
                    Fornavn = k.Fornavn,
                    Etternavn = k.Etternavn,
                    Epost = k.Epost,
                    Telefonnr = k.Telefonnr
                }).FirstOrDefaultAsync();
                if(hentetKunde == default)
                {
                    _log.LogInformation("BestillingRepository.cs: HentEnKunde: Feil kunde-id");
                }
                _log.LogInformation("BestillingReposuitory.cs: HentEnKunde: Vellykket. Kunde med id " + id + " hentet.");
                return hentetKunde;

            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentEnKunde: Feil i databasen. "+e+". Prøv igjen");
                return null;
            }
        }

        public async Task<List<Personer>> HentAlleKunder()
        {
            try
            {
                List<Personer> alleKunder = await _db.KunderOgAnsatte.Where(k => k.Admin == false).Select(k => new Personer()
                {
                    Id = k.Id,
                    Fornavn = k.Fornavn,
                    Etternavn = k.Etternavn,
                    Epost = k.Epost,
                    Telefonnr = k.Telefonnr
                }).ToListAsync();

                if (!alleKunder.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleKunder: Ingen kunder i databasen");
                }
                _log.LogInformation("BestillingRepository.cs: HentAlleKunder: Vellykket. Kunder hentet");
                return alleKunder;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleKunder: Feil i databasen: " + e + ". Kunder ikke hentet");
                return null;
            }
        }

        public async Task<Personer> HentEnAnsatt(int id)
        {
            try
            {
                Personer hentetAnsatt = await _db.KunderOgAnsatte.Where(a => a.Id == id && a.Admin == true).Select(a => new Personer()
                {
                    Id = a.Id,
                    Fornavn = a.Fornavn,
                    Etternavn = a.Etternavn,
                    Epost = a.Epost,
                    Telefonnr = a.Telefonnr
                }).FirstOrDefaultAsync();
                if (hentetAnsatt == default)
                {
                    _log.LogInformation("BestillingRepository.cs: HentEnAnsatt: Feil ansatt-id");
                }
                _log.LogInformation("BestillingReposuitory.cs: HentEnKunde: Vellykket. Ansatt med id " + id + " hentet.");
                return hentetAnsatt;

            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentEnAnsatt: Feil i databasen. " + e + ". Prøv igjen");
                return null;
            }
        }

        public async Task<List<Personer>> HentAlleAnsatte()
        {
            try
            {
                List<Personer> alleAnsatte = await _db.KunderOgAnsatte.Where(a => a.Admin == true).Select(a => new Personer()
                {
                    Id = a.Id,
                    Fornavn = a.Fornavn,
                    Etternavn = a.Etternavn,
                    Epost = a.Epost,
                    Telefonnr = a.Telefonnr
                }).ToListAsync();

                if (!alleAnsatte.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleAnsatte: Ingen ansatte i databasen");
                }
                _log.LogInformation("BestillingRepository.cs: HentAlleAnsatte: Vellykket. Ansatte hentet");
                return alleAnsatte;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleAnsatte: Feil i databasen: " + e + ". Ansatte ikke hentet");
                return null;
            }
        }

        // I utgangspunktet burde kundeinformasjon endres av kunden selv, men det kan være aktuelt for admin å endre kundeinformasjon dersom de f.eks. får
        // beskjed om navneendring fra folkeregisteret
        // Her kan også adminstatus gis til ansatte av admin.
        public async Task<bool> EndrePerson(Personer person)
        {
            try
            {
                Person somSkalEndres = await _db.KunderOgAnsatte.FindAsync(person.Id);
                if (!(somSkalEndres == null))
                {
                    somSkalEndres.Fornavn = person.Fornavn;
                    somSkalEndres.Etternavn = person.Etternavn;
                    somSkalEndres.Epost = person.Epost;
                    somSkalEndres.Telefonnr = person.Telefonnr;

                    await _db.SaveChangesAsync();
                    _log.LogInformation("BestillingRepository.cs: EndrePerson: Vellykket! Personalia endret");
                    return true;

                }
                else
                {
                    _log.LogInformation("BestillingRepository.cs: EndrePerson: Fant ikke personen i databasen");
                    return false;
                }
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: EndrePerson: Feil i databasen. Person ikke endret. " + e);
                return false;
            }
        }

        // En kunde med bestilling(er) kan ikke slettes. Dersom kunde likevel skal slettes må bestilling(er) håndteres først, og da med tilknyttede regler.

        public async Task<bool> SlettKunde(int id)
        {
            try
            {
                Person fjerneKunde = await _db.KunderOgAnsatte.FirstOrDefaultAsync(k => k.Id == id && k.Admin == false);
                if (!(fjerneKunde == default))
                {
                    List<Bestillinger> AlleBestillinger = await _db.Bestillinger.Where(b => b.Kunde.Id == id).ToListAsync();
                    if(!AlleBestillinger.Any())
                    {
                        _db.KunderOgAnsatte.Remove(fjerneKunde);
                        await _db.SaveChangesAsync();
                        _log.LogInformation("BestillingRepository.cs: SlettKunde: Vellykket! Kunde slettet");
                        return true;
                    }
                    _log.LogInformation("BestillingRepository.cs: SlettKunde: Kunden har bestillinger i databasen. Kunde ikke slettet");
                    return false;
                    
                }
                _log.LogInformation("BestillingRepository.cs: SlettKunde: Kunde finnes ikke i databasen");
                return false;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: SlettKunde: Feil i databasen. Kunde ikke slettet. " + e);
                return false;
            }
        }

        public async Task<int> LoggInn(Bruker bruker)
        {
            try
            {
                Brukere brukerIDB = await _db.Brukere.FirstOrDefaultAsync(b => b.Brukernavn.Equals(bruker.Brukernavn));
                if (brukerIDB == default(Brukere))
                {
                    _log.LogInformation("BestillingRepository.cs: LoggInn: Ingen bruker funnet i database med brukernavn " + bruker.Brukernavn);
                    return 0;
                }
                byte[] hash = LagEnHash(bruker.Passord, brukerIDB.Salt);
                bool OK = hash.SequenceEqual(brukerIDB.Passord);
                if (OK)
                {
                    _log.LogInformation("BestillingRepository.cs: LoggInn: Vellykket! Bruker logget inn");
                    return brukerIDB.Id;
                }
                _log.LogInformation("BestillingRepository.cs: LogInn: Logg inn feilet");
                return 0;

            } catch (Exception e) {
                _log.LogInformation("BestillingRepository.cs: LoggInn: Databasefeil: " + e);
                return 0;
            }
        }

        public async Task<Personer> HentProfil(string username)
        {
            try
            {
                Brukere bruker = await _db.Brukere.FirstOrDefaultAsync(b => b.Brukernavn.Equals(username));
                if(bruker != null)
                {
                    Personer brukerProfil = new Personer()
                    {
                        Id = bruker.Person.Id,
                        Fornavn = bruker.Person.Fornavn,
                        Etternavn = bruker.Person.Etternavn,
                        Epost = bruker.Person.Epost,
                        Telefonnr = bruker.Person.Telefonnr
                    };
                    _log.LogInformation("BestillingRepository.cs: HentProfil: Vellykket! Brukerprofil returnert");
                    return brukerProfil;
                }
                _log.LogInformation("BestillingRepository.cs: HentProfil: Fant ikke bruker i databasen");
                return null;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentProfil: Brukerprofil ikke hentet. Databasefeil: " + e);
                return null;
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
            List<Ferd> ferder = await _db.Ferder.Where(f => f.Rute.Id == ruteId).ToListAsync();
            System.Diagnostics.Debug.WriteLine(ferder.Count);
            if (!ferder.Any())
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentFerder: Ingen ferder ble returnert fra databasen.");
                return null;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentFerder: Vellykket. Ferdene ble returnert fra databasen.");
            return ferder;
        }

        public async Task<bool> LagreBestilling(Bestilling bestilling)
        {
            try
            {
                Person kunde = await _db.KunderOgAnsatte.FindAsync(bestilling.KId);
                if(kunde != null)
                {
                    Bestillinger nyBestilling = new Bestillinger
                    {
                        Kunde = kunde,
                        Betalt = bestilling.Betalt,
                    };
                    _db.Bestillinger.Add(nyBestilling);
                    await _db.SaveChangesAsync();
                    _log.LogInformation("BestillingRepository.cs: LagreBestilling: Vellykket! Bestilling lagret i databasen.");
                    return true;
                }
                _log.LogInformation("BestillingRepository.cs: LagreBestilling: Bestilling ikke lagret! Kunde ikke funnet");
                return false;
                
            } catch(Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: LagreBestilling: Databasefeil: " + e + ". Bestilling ikke lagret");
                return false;
            }
        }

        public async Task<string> LagreBestilling(BestillingOutput nyBestilling)
        {
            double totalPris = 0.00;
            List<Billett> billettListe = new List<Billett>();

            DateTime AvreiseTid = ParseDatoLocal(nyBestilling.AvreiseTid);
            
            Ferd ferd = await _db.Ferder.FirstOrDefaultAsync(f => f.AvreiseTid.Date.Equals(UtenTimer(AvreiseTid.Date)) &&
                f.Rute.Startpunkt.Equals(nyBestilling.Startpunkt) && f.Rute.Endepunkt.Equals(nyBestilling.Endepunkt));

            Ferd ferdRetur;
            // Hvis HjemreiseTid-parameteren er definert, blir det antatt at bestillingen er en tur/retur bestilling.
            if (nyBestilling.HjemreiseTid != null)
            {
                DateTime HjemreiseTid = ParseDatoLocal(nyBestilling.HjemreiseTid);
                ferdRetur = await _db.Ferder.FirstOrDefaultAsync(f => f.AvreiseTid.Date.Equals(UtenTimer(HjemreiseTid.Date)) &&
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
                _log.LogInformation("/Controllers/BestillingRepository.cs: LagreBestilling: Ferd med passende dato, og de samme " +
                    "start- og endepunktene har blitt funnet i databasen.");
                Billett nyBillett;
                for (int i = 1; i <= nyBestilling.AntallVoksne; i++)
                {
                    nyBillett = new Billett()
                    {
                        Ferd = ferd,
                        Voksen = true,
                    };
                    billettListe.Add(nyBillett);

                    // Hvis fer
                    if (ferdRetur != null)
                    {
                        Billett returBillett = new Billett()
                        {
                            Ferd = ferdRetur,
                            Voksen = true
                        };
                        billettListe.Add(returBillett);
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
                            Voksen = false
                        };
                        billettListe.Add(returBillett);
                    }

                }

                //Oppretter bestillingen
                Bestillinger bestilling = new Bestillinger()
                {
                    Billetter = billettListe
                };


                _db.Bestillinger.Add(bestilling);
                await _db.SaveChangesAsync();
                return bestilling.Id + "";
            }
            else
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: LagreBestilling: Ferd med passende dato, og de samme start- og endepunktene har ikke" +
                    " blitt funnet i databasen.");
                return null;
            }
        }

        public async Task<BestillingOutput> HentBestilling(int id)
        {
            BestillingOutput bestilling = await _db.Bestillinger.Where(b => b.Id == id).Select(b => new BestillingOutput
            {
                Startpunkt = b.Billetter.First().Ferd.Rute.Startpunkt,
                Endepunkt = b.Billetter.First().Ferd.Rute.Endepunkt,
                AvreiseTid = b.Billetter.First().Ferd.AvreiseTid.ToString("o"),
                HjemreiseTid = b.Billetter.FirstOrDefault(bi => bi.Ferd.Id != b.Billetter.First().Ferd.Id).Ferd.AvreiseTid.ToString("o"),
                AntallVoksne = b.Billetter.Where(bi => bi.Ferd.AvreiseTid.Equals(b.Billetter.First().Ferd.AvreiseTid) && bi.Voksen == true).Count(),
                AntallBarn = b.Billetter.Where(bi => bi.Ferd.AvreiseTid.Equals(b.Billetter.First().Ferd.AvreiseTid) && bi.Voksen == false).Count()
            }).FirstOrDefaultAsync();

            if (bestilling != default(BestillingOutput))
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentBestilling: Vellykket. Et BestillingInput objekt blir returnert.");
                return bestilling;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentBestilling: Ingen bestilling med ID " + id + " har blitt funnet i databasen");
            return bestilling;
        }


        public async Task<double> HentPris(int id)
        {
            Bestillinger bestilling = await _db.Bestillinger.FirstOrDefaultAsync(b => b.Id == id);
            double pris = 0.0;
            if(bestilling != default)
            {
                pris = await BeregnTotalPris(id);
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentPris: Vellykket. Totalpris for bestilling med id " + id + " har blitt funnet i databasen.");
                return pris;
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentPris: TotalPris ikke funnet for bestilling med id " + id);
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
                DateTime AnkomstTid = await _db.Ferder.Where(f => f.Rute.Startpunkt.Equals(Startpunkt) && f.Rute.Endepunkt.Equals(Endepunkt) && f.AvreiseTid.Date.Equals(ParseDatoLocal(AvreiseTid))).Select(f => f.AnkomstTid).FirstOrDefaultAsync();
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
                Datoer[i] = UtenTimer(Datoer[i]);
            }
            _log.LogInformation("/Controllers/BestillingRepository.cs: HentDatoer: Vellykket. Ferd med Startpunkt '" + Startpunkt + "' og Endepunkt '" + Endepunkt + "' har blitt funnet i databasen.");
            return Datoer;
        }

        public async Task<string> HentAnkomstTid(int id, string Startpunkt)
        {
            DateTime AnkomstTid = await _db.Bestillinger.Where(b => b.Id == id).SelectMany(b => b.Billetter).Where(bi => bi.Ferd.Rute.Startpunkt.Equals(Startpunkt)).Select(bi => bi.Ferd.AnkomstTid).FirstOrDefaultAsync();
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
            string Baatnavn = await _db.Bestillinger.Where(b => b.Id == id).SelectMany(b => b.Billetter).Where(bi => bi.Ferd.Rute.Startpunkt.Equals(Startpunkt)).Select(bi => bi.Ferd.Baat.Navn).FirstOrDefaultAsync();
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
            List<Billett> billetter = await _db.Bestillinger.Where(b => b.Id == id).SelectMany(b => b.Billetter).Where(bi => bi.Ferd.Rute.Startpunkt.Equals(Startpunkt)).ToListAsync();
            
            if (!billetter.Any())
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentStrekningsPris: Enten ingen billetter med startpunkt " + Startpunkt + " eller ingen bestilling med ID " + id + " har blitt funnet i databasen.");
                return 0.00;
            } else
            {
                int antBarn = billetter.Count(b => !b.Voksen);
                int antVoksne = billetter.Count(b => b.Voksen);
                double prisPerBillett = billetter.First().Ferd.Rute.Pris;
                strekningsPris += ((antBarn * 0.5 * prisPerBillett) + (antVoksne * prisPerBillett));
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentStrekningsPris: Vellykket. Strekningspris " + strekningsPris + " funnet for " + antBarn + " barn og " + antVoksne + " voksne fra avreisehavn " + Startpunkt + " for bestilling med id " + id);
                return strekningsPris;
            }
            
        }

        // En lokal metode for å konvertere dato strenger til DateTime objekter
        private DateTime ParseDatoLocal(string dato_tid)
        {
            DateTime dato = DateTime.Parse(dato_tid, null, System.Globalization.DateTimeStyles.AssumeLocal);
            return dato; 
        }

        private DateTime UtenTimer(DateTime dato)
        {
            return new DateTime(dato.Year, dato.Month, dato.Day, 0, 0 ,0); 
        }

        public static byte[] LagEnHash(string passord, byte[] salt)
        {
            return KeyDerivation.Pbkdf2(
                password: passord,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 1500,
                numBytesRequested: 32);
        }

        public static byte[] LagEtSalt()
        {
            var csp = new RNGCryptoServiceProvider();
            var salt = new byte[24];
            csp.GetBytes(salt);
            return salt;
        }

        // Beregner totalpris. Returbilletter har rabatt
        private async Task<double> BeregnTotalPris(int id)
        {
            double TotalPris = 0.0;
            List<Billett> billetter = await _db.Billetter.Where(b => b.Bestilling.Id == id).ToListAsync();
            if (billetter.Any())
            {
                string Startpunkt = billetter.First().Ferd.Rute.Startpunkt;
                foreach (Billett billett in billetter)
                {
                    if (billett.Ferd.Rute.Startpunkt.Equals(Startpunkt))
                    {
                        if (billett.Voksen) TotalPris += (billett.Ferd.Rute.Pris);
                        else TotalPris += (0.5 * billett.Ferd.Rute.Pris);
                    }
                    else
                    {
                        if (billett.Voksen) TotalPris += (0.75 * billett.Ferd.Rute.Pris);
                        else TotalPris += (0.75 * 0.5 * billett.Ferd.Rute.Pris);
                    }
                }
            }
            return TotalPris;
        }

    }
}
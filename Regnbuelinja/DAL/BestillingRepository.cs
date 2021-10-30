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
                    Startpunkt = rute.Avreisehavn,
                    Endepunkt = rute.Ankomsthavn,
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
                    
                    somSkalEndres.Startpunkt = endreRute.Avreisehavn;
                    somSkalEndres.Endepunkt = endreRute.Ankomsthavn;
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

        public async Task<bool> SlettRute(int id)
        {
            try
            {
                Rute fjerneRute = await _db.Ruter.FindAsync(id);
                if(!(fjerneRute == null))
                {
                    List<Billett> AlleRelaterteBilletter = await _db.Billetter.Where(b => b.Ferd.Rute.Id == fjerneRute.Id).ToListAsync();
                    if(AlleRelaterteBilletter.Any())
                    {
                        _log.LogInformation("BestillingRepository.cs: SlettRute: Rute med i en bestillt ferd. Ikke slettet");
                        return false;
                    }
                    List<Ferd> AlleRelaterteFerder = await _db.Ferder.Where(f => f.Rute.Id == id).ToListAsync();
                    if(AlleRelaterteFerder.Any())
                    {
                        foreach (Ferd ferd in AlleRelaterteFerder)
                        {
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
                if(hentetRute == default)
                {
                    _log.LogInformation("BestillingRepository.cs: HentEnRute: Rute ikke funnet i databasen");
                }
                return hentetRute;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepisotory.cs: HentEnRute: Feil i databasen på serveren: "+e);
                return null;
            }
        }

        public async Task<List<Rute>> HentAlleRuter()
        {
            try
            {
                List<Rute> alleRutene = await _db.Ruter.ToListAsync();
                if(!alleRutene.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleRuter: Ingen ruter i databasen");
                }
                _log.LogInformation("BestillingRepository.cs: HentAlleRuter: Vellykket! Ruter hentet");
                return alleRutene;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleRuter: Databasefeil: " + e +". Ruter ikke hentet");
                return null;
            }
            
        }

        public async Task<bool> LagreBåt(Baater båt) {
            try
            {
                Baat nyBaat = new Baat
                {
                    Navn = båt.Båtnavn
                };

                _db.Baater.Add(nyBaat);
                _log.LogInformation("BestillingRepository.cs: LagreBåt: Vellykket! Båt lagret i databasen");
                await _db.SaveChangesAsync();
                return true;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: LagreBåt: Feil i databasen: " + e +". Båt ikke lagret");
                return false;
            }
            
        }

        //Båt i bestilling kan endres, men ikke slettes - må oppdateres hvis vi implementerer kapasitet.
        public async Task<bool> EndreBåt(Baater båt)
        {
            try
            {
                Baat somSkalEndres = await _db.Baater.FirstOrDefaultAsync(b => b.Id == båt.Id);
                if(somSkalEndres!= default)
                {
                    somSkalEndres.Navn = båt.Båtnavn;
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

        public async Task<bool> SlettBåt(int id)
        {
            try
            {
                Baat somSkalSlettes = await _db.Baater.FirstOrDefaultAsync(b => b.Id == id);
                if(somSkalSlettes != default)
                {
                    List<Billett> AlleRelaterteBilletter = await _db.Billetter.Where(b => b.Ferd.Baat.Id == somSkalSlettes.Id).ToListAsync();
                    if (AlleRelaterteBilletter.Any())
                    {
                        _log.LogInformation("BestillingRepository.cs: SlettBåt: Båt med i en bestillt ferd. Ikke slettet");
                        return false;
                    }
                    List<Ferd> AlleRelaterteFerder = await _db.Ferder.Where(f => f.Rute.Id == id).ToListAsync();
                    if (AlleRelaterteFerder.Any())
                    {
                        foreach (Ferd ferd in AlleRelaterteFerder)
                        {
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
                _log.LogInformation("BestillingRepository.cs: SlettBåt: Feil i databasen: "+e+". Båt ikke slettet");
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
                if(hentetBåt == default)
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
                if(alleFerdene.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleFerder: Vellykket! Ferder hentet");
                }
                
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
                    if(nyRute == default || nyBåt == default)
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
            } catch(Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: EndreFerd: Databasefeil: " + e + ". Ferd ikke endret");
                return false;
            }
        }

        public async Task<bool> SlettFerd(int id)
        {
            try
            {
                Ferd fjerneFerd = await _db.Ferder.FirstOrDefaultAsync(f => f.Id == id);
                if (!(fjerneFerd == default))
                {
                    List<Billett> AlleRelaterteBilletter = await _db.Billetter.Where(b => b.Ferd.Id == fjerneFerd.Id).ToListAsync();
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
                List<Bestilling> alleBestillinger = await _db.Bestillinger.Select(b => new Bestilling()
                {
                    Id = b.Id,
                    KId = b.Kunde.Id,
                    Totalpris = b.TotalPris,
                    Betalt = b.Betalt
                }).ToListAsync();
                if (!alleBestillinger.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleBestillinger: Ingen bestillinger i databasen");
                }
                return alleBestillinger;
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
                Bestilling bestilling = await _db.Bestillinger.Select(b => new Bestilling()
                {
                    Id = b.Id,
                    KId = b.Kunde.Id,
                    Totalpris = b.TotalPris,
                    Betalt = b.Betalt
                }).FirstOrDefaultAsync();
                if (bestilling == default)
                {
                    _log.LogInformation("BestillingRepository.cs: HentEnBestilling: Ingen bestilling med id "+id+" i databasen");
                }
                return bestilling;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentEnBestilling: Feil i databasen: " + e + ". Bestilling ikke hentet");
                return null;
            }
        }

        // En bestilling kan slettes (etter kundeønske) hvis den ikke er betalt enda eller hvis ferden allerede er gjennomført (ankomsttid er tidligere enn dagens dato).
        // TROR DENNE FUNKER! MÅ TESTES!!!
        public async Task<bool> SlettBestilling(int id)
        {
            try
            {
                Bestillinger somSkalSlettes = await _db.Bestillinger.FindAsync(id);
                if(somSkalSlettes != null)
                {
                    Ferd ferd = somSkalSlettes.Billetter.First().Ferd;
                    Ferd returFerd = somSkalSlettes.Billetter.Where(b => b.Ferd.Id != ferd.Id).Select(b => b.Ferd).FirstOrDefault();
                    DateTime ankomstTid = returFerd == default ? ferd.AnkomstTid : returFerd.AnkomstTid;
                    if ((!somSkalSlettes.Betalt) || (ankomstTid.CompareTo(DateTime.Now) < 0))
                    {
                        foreach (Billett billett in somSkalSlettes.Billetter)
                        {
                            _db.Billetter.Remove(billett);
                        }
                        _log.LogInformation("BestillingRepository.cs: SlettBestilling: Vellykket. Bestilling slettet");
                        _db.Remove(somSkalSlettes);
                        await _db.SaveChangesAsync();
                    }
                    _log.LogInformation("BestillingRepository.cs: SlettBestilling: Bestillingen er ikke betalt enda. Kan ikke slettes");
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

        // En ubetalt bestilling kan endres (ved kundeønske), mens en betalt bestilling er fastsatt og kan ikke endres (faktureringsavdeling tar seg av dette, vi vil holde
        // logikken så enkel som mulig).

        public async Task<bool> EndreBestilling(Bestilling bestilling)
        {
            try
            {
                Bestillinger endreBestilling = await _db.Bestillinger.FindAsync(bestilling.Id);
                if (endreBestilling != null)
                {
                    if (endreBestilling.Betalt != true)
                    {
                        Kunde nyKunde = await _db.Kunder.FirstOrDefaultAsync(k => k.Id == bestilling.KId); 
                        if(nyKunde != default)
                        {
                            endreBestilling.Kunde = nyKunde;
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

        //SE PÅ DENNE LOGIKKEN IGJEN! Det som skal være tilfelle: En bestilling som ikke er betalt kan slettes, med mindre den allerede har skjedd. En betalt billett kan slettes hvis den har vært
        public async Task<bool> EndreBillett(Billetter nyBillett)
        {
            try
            {
                Billett somSkalEndres = await _db.Billetter.FindAsync(nyBillett.Id);
                if (somSkalEndres != null)
                {
                    if (!somSkalEndres.Bestilling.Betalt || (somSkalEndres.Ferd.AnkomstTid.CompareTo(DateTime.Now) < 0))
                    {
                        Ferd endreFerd = await _db.Ferder.FindAsync(nyBillett.FId);
                        if (endreFerd != null)
                        {
                            somSkalEndres.Ferd = endreFerd;
                            somSkalEndres.Voksen = nyBillett.Voksen;
                            await _db.SaveChangesAsync();
                            _log.LogInformation("BestillingRepository.cs: EndreBillett: Vellykket. Billetten er endret");
                            return true;
                        }
                    }
                    _log.LogInformation("BestillingRepository.cs: EndreBillett: Bestilling allerede betalt men reisen ikke foretatt eller billett ikke betalt, men reise gjennomført");
                }
                _log.LogInformation("BestillingRepository.cs: EndreBillett: Billett ikke funnet i databasen");
                return false;
            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: EndreBillett: Databasefeil. Prøv igjen");
                return false;
            }
        }

        //Billett kan slettes dersom ankomstTid har vært og billett er betalt ELLER ankomsttid ikke har vært, MEN betalingen ikke er gjennomført.
        public async Task<bool> SlettBillett(int id)
        {
            try
            {
                Billett somSkalSlettes = await _db.Billetter.FindAsync(id);
                if (somSkalSlettes != null)
                {
                    DateTime ankomstTid = somSkalSlettes.Ferd.AnkomstTid;
                    if ((!somSkalSlettes.Bestilling.Betalt) || (ankomstTid.CompareTo(DateTime.Now)<0))
                    {
                        _log.LogInformation("BestillingRepository.cs: SlettBestilling: Vellykket. Bestilling slettet");
                        _db.Remove(somSkalSlettes);
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

        //vet ikke om trengs
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
                List<Billetter> alleBilletter = await _db.Billetter.Where(b => b.Id == id).Select(b => new Billetter()
                {
                    Id = b.Id,
                    FId = b.Ferd.Id,
                    BId = b.Bestilling.Id,
                    Voksen = b.Voksen
                }).ToListAsync();
                if(!alleBilletter.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForBestilling: Bestilling ikke funnet eller ingen billetter i bestilling");
                }
                return alleBilletter;
            } catch(Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForBestilling: Feil i databasen: " +e+". Billetter ikke hentet");
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
                    FId = b.Ferd.Id,
                    BId = b.Bestilling.Id,
                    Voksen = b.Voksen
                }).Distinct().ToListAsync();
                
                if (!alleBilletter.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForFerd: Ferd ikke funnet eller ingen billetter for ferd");
                }
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
                }).Distinct().ToListAsync();

                if (!alleBilletter.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForRute: Rute ikke funnet eller ingen billetter for rute");
                }
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
                    FId = b.Ferd.Id,
                    BId = b.Bestilling.Id,
                    Voksen = b.Voksen
                }).Distinct().ToListAsync();

                if (!alleBilletter.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForBåt: Båt ikke funnet eller ingen billetter for båt");
                }
                return alleBilletter;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleBilletterForBåt: Feil i databasen: " + e + ". Billetter ikke hentet");
                return null;
            }
        }

        public async Task<List<Bestilling>> HentBestillingerForKunde(int id)
        {
            try
            {
                List<Bestilling> alleBestillinger = await _db.Bestillinger.Where(b => b.Kunde.Id == id).Select(b => new Bestilling()
                {
                    Id = b.Id,
                    KId = b.Kunde.Id,
                    Totalpris = b.TotalPris,
                    Betalt = b.Betalt
                }).Distinct().ToListAsync();

                if (!alleBestillinger.Any())
                {
                    _log.LogInformation("BestillingRepository.cs: HentBestillingerForKunde: Kunde ikke funnet eller kunden har ikke bestillt neon reise enda");
                }
                return alleBestillinger;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentBestillingerForKunde: Feil i databasen: " + e + ". Bestillinger ikke hentet");
                return null;
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
            } catch(Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: LagreBruker: Bruker ikke lagret. Databasefeil: " + e);
                return false;
            }
        }

        public async Task<bool> LagreKunde(Kunder kunde)
        {
            try
            {
                Kunde NyKunde = new Kunde()
                {
                    Fornavn = kunde.Fornavn,
                    Etternavn = kunde.Etternavn,
                    Epost = kunde.Epost,
                    Telefonnr = kunde.Telefonnr
                };

                _db.Kunder.Add(NyKunde);
                await _db.SaveChangesAsync();
                _log.LogInformation("BestillingRepository.cs: LagreKunde: Kunde lagret");
                return true;
            } catch(Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: LagreKunde: Feil i databasen: " + e + ". Kunde ikke lagret");
                return false;
            }
        }

        public async Task<Kunder> HentEnKunde(int id)
        {
            try
            {
                Kunder hentetKunde = await _db.Kunder.Where(k => k.Id == id).Select(k => new Kunder()
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
                _log.LogInformation("BestillingRepository.cs: HentEnKunde: Feil i databasen. Prøv igjen");
                return null;
            }
        }

        public async Task<List<Kunder>> HentAlleKunder()
        {
            try
            {
                List<Kunder> alleKunder = await _db.Kunder.Select(k => new Kunder()
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
                return alleKunder;
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: HentAlleKunder: Feil i databasen: " + e + ". Kunder ikke hentet");
                return null;
            }
        }

        public async Task<bool> EndreKunde(Kunder kunde)
        {
            try
            {
                Kunde somSkalEndres = await _db.Kunder.FindAsync(kunde.Id);
                if (!(somSkalEndres == null))
                {
                    somSkalEndres.Fornavn = kunde.Fornavn;
                    somSkalEndres.Etternavn = kunde.Etternavn;
                    somSkalEndres.Epost = kunde.Epost;
                    somSkalEndres.Telefonnr = kunde.Telefonnr;

                    await _db.SaveChangesAsync();
                    _log.LogInformation("BestillingRepository.cs: EndreKunde: Vellykket! Kunde endret");
                    return true;

                }
                else
                {
                    _log.LogInformation("BestillingRepository.cs: EndreKunde: Kunde finnes ikke i databasen");
                    return false;
                }
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: EndreKunde: Feil i databasen. Kunde ikke endret. " + e);
                return false;
            }
        }

        //Kan kun slette kunde dersom kunde ikke har ubetalte bestillinger
        public async Task<bool> SlettKunde(int id)
        {
            try
            {
                Kunde fjerneKunde = await _db.Kunder.FirstOrDefaultAsync(k => k.Id == id);
                if (!(fjerneKunde == default))
                {
                    List<Bestillinger> AlleRelaterteBestillinger = await _db.Bestillinger.Where(b => (b.Kunde.Id == id && b.Betalt == false)).ToListAsync();
                    if (AlleRelaterteBestillinger.Any())
                    {
                        _log.LogInformation("BestillingRepository.cs: SlettKunde: Kunde har ubetalte bestillinger. Ikke slettet.");
                        return false;
                    }

                    _db.Kunder.Remove(fjerneKunde);
                    await _db.SaveChangesAsync();
                    _log.LogInformation("BestillingRepository.cs: SlettKunde: Vellykket! Kunde slettet");
                    return true;
                }
                else
                {
                    _log.LogInformation("BestillingRepository.cs: SlettKunde: Kunde finnes ikke i databasen");
                    return false;
                }
            }
            catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: SlettKunde: Feil i databasen. Kunde ikke slettet. " + e);
                return false;
            }
        }

        public async Task<bool> LoggInn(Bruker bruker)
        {
            try
            {
                Brukere brukerIDB = await _db.Brukere.FirstOrDefaultAsync(b => b.Brukernavn.Equals(bruker.Brukernavn));
                if (brukerIDB == default(Brukere))
                {
                    _log.LogInformation("BestillingRepository.cs: LoggInn: Ingen bruker funnet i database med brukernavn " + bruker.Brukernavn);
                    return false;
                }
                byte[] hash = LagEnHash(bruker.Passord, brukerIDB.Salt);
                bool OK = hash.SequenceEqual(brukerIDB.Passord);
                if (OK)
                {
                    return true;
                }
                return false;

            } catch (Exception e) {
                _log.LogInformation("BestillingRepository.cs: LoggInn: Ikke logget inn. Feil: " + e);
                return false;
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
                return bestilling.Id + "";
            }
            else
            {
                _log.LogInformation("/Controllers/BestillingRepository.cs: LagreBestilling: Ferd med passende dato, og de samme start- og endepunktene har ikke blitt funnet i databasen.");
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
            double pris = await _db.Bestillinger.Where(b => b.Id == id).Select(b => b.TotalPris).FirstOrDefaultAsync();
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
                strekningsPris += (antBarn * 0.5 * prisPerBillett) + (antVoksne * prisPerBillett);
                _log.LogInformation("/Controllers/BestillingRepository.cs: HentStrekningsPris: Vellykket. Strekningspris " + strekningsPris + " funnet for " + antBarn + " barn og " + antVoksne + " voksne fra avreisehavn " + Startpunkt + " for bestilling med id " + id);
                return strekningsPris;
            }
            
        }

        public async Task<bool> Betal(int id)
        {
            try
            {
                Bestillinger best = await _db.Bestillinger.FindAsync(id);
                if (best != null)
                {
                    best.Betalt = true;
                    await _db.SaveChangesAsync();
                    _log.LogInformation("BestillingRepositor.cs: Betal: Vellykket: Bestillingen er betalt");
                    return true;
                }
                _log.LogInformation("BestillingRepository.cs: Betal: Fant ikke bestillingen i databasen");
                return false;

            } catch (Exception e)
            {
                _log.LogInformation("BestillingRepository.cs: Betal: Databasefeil: " + e + ". Betalingen ikke gjennomført");
                return false;
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

    }
}
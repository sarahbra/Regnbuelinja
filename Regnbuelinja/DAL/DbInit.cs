using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Regnbuelinja.Models;

namespace Regnbuelinja.DAL
{
    public class DbInit
    {
        public static void Initialize(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<BestillingContext>();

                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Oslo - Kobenhavn
                var rute1 = new Rute { Startpunkt = "Oslo", Endepunkt = "København", Pris = 799 };
                var rute2 = new Rute { Startpunkt = "København", Endepunkt = "Oslo", Pris = 799 };

                // Oslo - Kiel
                var rute3 = new Rute { Startpunkt = "Oslo", Endepunkt = "Kiel", Pris = 399 };
                var rute4 = new Rute { Startpunkt = "Kiel", Endepunkt = "Oslo", Pris = 399 };

                // Kobenhavn - Kiel
                var rute5 = new Rute { Startpunkt = "København", Endepunkt = "Kiel", Pris = 299 };
                var rute6 = new Rute { Startpunkt = "Kiel", Endepunkt = "København", Pris = 299 };

                // Bergen - Kobenhavn
                var rute7 = new Rute { Startpunkt = "Bergen", Endepunkt = "København", Pris = 1199 };
                var rute8 = new Rute { Startpunkt = "København", Endepunkt = "Bergen", Pris = 1199 };

                // Oslo - Bergen
                var rute9 = new Rute { Startpunkt = "Oslo", Endepunkt = "Bergen", Pris = 699 };
                var rute10 = new Rute { Startpunkt = "Bergen", Endepunkt = "Oslo", Pris = 699 };


                var baat1 = new Baat { Navn = "Båtten Anna" };
                var baat2 = new Baat { Navn = "Unicorn" };
                var baat3 = new Baat { Navn = "Heisann" };
                var baat4 = new Baat { Navn = "ILikeBigBoats" };
                var baat5 = new Baat { Navn = "SheGotTheHouse" };
                var baat6 = new Baat { Navn = "HMK Pride" };
                var baat7 = new Baat { Navn = "Boaty McBoatface" };
                var baat8 = new Baat { Navn = "The Queen" };


                var ferd1 = new Ferd { Baat = baat1, Rute = rute1, AvreiseTid = new DateTime(2021, 12, 1, 13, 0, 0), AnkomstTid = new DateTime(2021, 12, 2, 11, 0, 0) };
                var ferd2 = new Ferd { Baat = baat2, Rute = rute3, AvreiseTid = new DateTime(2021, 12, 23, 14, 0, 0), AnkomstTid = new DateTime(2021, 12, 28, 17, 0, 0) };
                var ferd3 = new Ferd { Baat = baat8, Rute = rute10, AvreiseTid = new DateTime(2021, 12, 4, 9, 0, 0), AnkomstTid = new DateTime(2021, 12, 10, 19, 30, 0) };
                var ferd4 = new Ferd { Baat = baat3, Rute = rute4, AvreiseTid = new DateTime(2021, 12, 6, 12, 30, 30), AnkomstTid = new DateTime(2021, 12, 9, 2, 22, 30, 0)};
                


                //avreisedatoer for rute1, 5, og 9
                for (int i = 1; i < 31; i ++)
                {
                    if (i % 2 == 0)
                    {
                        context.Ferder.Add(new Ferd { Baat = baat1, Rute = rute1, AvreiseTid = new DateTime(2021, 12, i, 13, 25, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 18, 0, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat8, Rute = rute5, AvreiseTid = new DateTime(2021, 12, i, 10, 45, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 12, 0, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat4, Rute = rute9, AvreiseTid = new DateTime(2021, 12, i, 12, 10, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 11, 05, 0) });
                    }
                    else
                    {
                        context.Ferder.Add(new Ferd { Baat = baat2, Rute = rute1, AvreiseTid = new DateTime(2021, 12, i, 8, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 16, 0, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat7, Rute = rute5, AvreiseTid = new DateTime(2021, 12, i, 11, 25, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 14, 25, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat3, Rute = rute9, AvreiseTid = new DateTime(2021, 12, i, 11, 00, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 13, 35, 0) });
                    }
                }

                //avreisedatoer for rute2, 6, og 10
                for (int i = 1; i < 31; i++)
                {
                    if (i % 2 == 0)
                    {
                        context.Ferder.Add(new Ferd { Baat = baat3, Rute = rute2, AvreiseTid = new DateTime(2021, 12, i, 11, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 12, 0, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat6, Rute = rute6, AvreiseTid = new DateTime(2021, 12, i, 9, 15, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 11, 30, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat8, Rute = rute10, AvreiseTid = new DateTime(2021, 12, i, 6, 55, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 9, 25, 0) });
                    }
                    else
                    {
                        context.Ferder.Add(new Ferd { Baat = baat4, Rute = rute2, AvreiseTid = new DateTime(2021, 12, i, 10, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 15, 0, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat5, Rute = rute6, AvreiseTid = new DateTime(2021, 12, i, 8, 45, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 20, 0, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat1, Rute = rute10, AvreiseTid = new DateTime(2021, 12, i, 12, 00, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 12, 55, 0) });
                    }

                }

                //avreisedatoer for rute 3 og 7
                for (int i = 1; i < 30; i++)
                {
                    if (i % 2 == 0)
                    {
                        context.Ferder.Add(new Ferd { Baat = baat5, Rute = rute3, AvreiseTid = new DateTime(2021, 12, i, 6, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 12, 0, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat4, Rute = rute7, AvreiseTid = new DateTime(2021, 12, i, 6, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 12, 0, 0) });
                    }
                    else
                    {
                        context.Ferder.Add(new Ferd { Baat = baat6, Rute = rute3, AvreiseTid = new DateTime(2021, 12, i, 8, 50, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 14, 0, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat3, Rute = rute7, AvreiseTid = new DateTime(2021, 12, i, 6, 45, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 13, 15, 0) });
                    }
                }

                //avreisedatoer for rute 4 og 8
                for (int i = 1; i < 30; i++)
                {
                    if (i % 2 == 0)
                    {
                        context.Ferder.Add(new Ferd { Baat = baat7, Rute = rute4, AvreiseTid = new DateTime(2021, 12, i, 7, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 17, 0, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat2, Rute = rute8, AvreiseTid = new DateTime(2021, 12, i, 11, 25, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 17, 35, 0) });
                    }
                    else
                    {
                        context.Ferder.Add(new Ferd { Baat = baat8, Rute = rute4, AvreiseTid = new DateTime(2021, 12, i, 9, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 21, 0, 0) });
                        context.Ferder.Add(new Ferd { Baat = baat1, Rute = rute8, AvreiseTid = new DateTime(2021, 12, i, 8, 45, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 15, 45, 0) });
                    }
                }

                //Kun bestilling og billett til ferd1 foreløpig, kun 1 bestilling men 2 ruter, 2 båter og to mulige ferder

                var billett1 = new Billett { Ferd = ferd1, Voksen = true };
                var billett2 = new Billett { Ferd = ferd1, Voksen = false };
                var billett3 = new Billett { Ferd = ferd2, Voksen = true };
                var billett4 = new Billett { Ferd = ferd3, Voksen = false };
                var billett5 = new Billett { Ferd = ferd4, Voksen = true };
                

                List<Billett> billetter = new List<Billett>();
                billetter.Add(billett1);
                billetter.Add(billett2);
                

                List<Billett> billetter2 = new List<Billett>();
                billetter2.Add(billett3);
                billetter2.Add(billett4);

                List<Billett> billetter3 = new List<Billett>();
                billetter3.Add(billett5);


                Person kunde1 = new Person
                {
                    Fornavn = "Chuck",
                    Etternavn = "Norris",
                    Telefonnr = "22222222",
                    Epost = "kunde@mail.no"
                };
                Person kunde3 = new Person
                {
                    Fornavn = "Justin",
                    Etternavn = "Case",
                    Telefonnr = "99887766",
                    Epost = "justincase@icloud.com"
                };
                Person kunde4 = new Person
                {
                    Fornavn = "Curley",
                    Etternavn = "Tubes",
                    Telefonnr = "69696969",
                    Epost = "curly@gmail.com"
                };
                Person kunde5 = new Person
                {
                    Fornavn = "McDonald",
                    Etternavn = "Berger",
                    Telefonnr = "87778888",
                    Epost = "deilicious@hotmail.com"
                };
                Person kunde6 = new Person
                {
                    Fornavn = "Moe",
                    Etternavn = "Lester",
                    Telefonnr = "44445555",
                    Epost = "okidoki@outlook.com"
                };
                Person kunde7 = new Person
                {
                    Fornavn = "Trude",
                    Etternavn = "Lutt",
                    Telefonnr = "12345678",
                    Epost = "trude@sss.org"
                };
                Person kunde8 = new Person
                {
                    Fornavn = "Lydia",
                    Etternavn = "Nalen",
                    Telefonnr = "99999999",
                    Epost = "nalenmin@wow.no"
                };


                Person ansatt1 = new Person
                {
                    Fornavn = "Per",
                    Etternavn = "Hansen",
                    Telefonnr = "22232323",
                    Epost = "ansatt@mail.no",
                    Admin = true
                };

                // Adminbruker
                var Bruker = new Bruker()
                {
                    Brukernavn = "admin",
                    Passord = "Test1234"
                };

                var nyBruker = new Brukere();

                string passord = Bruker.Passord;
                byte[] salt = BestillingRepository.LagEtSalt();
                byte[] hash = BestillingRepository.LagEnHash(passord, salt);

                nyBruker.Brukernavn = Bruker.Brukernavn;
                nyBruker.Passord = hash;
                nyBruker.Salt = salt;
                nyBruker.Person = ansatt1;

                context.Brukere.Add(nyBruker);

                context.KunderOgAnsatte.Add(kunde1);
                context.KunderOgAnsatte.Add(kunde3);
                context.KunderOgAnsatte.Add(kunde4);
                context.KunderOgAnsatte.Add(kunde5);
                context.KunderOgAnsatte.Add(kunde6);
                context.KunderOgAnsatte.Add(kunde7);
                context.KunderOgAnsatte.Add(kunde8);
                context.KunderOgAnsatte.Add(ansatt1);

                //Hardkodet totalpris
                var bestilling1 = new Bestillinger { Billetter = billetter, Kunde = kunde1 };
                var bestilling2 = new Bestillinger { Billetter = billetter2, Kunde = kunde4 };
                var bestilling3 = new Bestillinger { Billetter = billetter3, Kunde = kunde3 };

                context.Bestillinger.Add(bestilling1);
                context.Bestillinger.Add(bestilling2);
                context.Bestillinger.Add(bestilling3);
                context.SaveChanges();
            }
        }
    }
}

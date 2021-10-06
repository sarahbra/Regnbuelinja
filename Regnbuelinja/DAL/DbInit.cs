using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

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


                var baat1 = new Baat { Navn = "Båtten Anna" };
                var baat2 = new Baat { Navn = "Unicorn" };
                var baat3 = new Baat { Navn = "Heisann" };
                var baat4 = new Baat { Navn = "ILikeBigBoats" };
                var baat5 = new Baat { Navn = "SheGotTheHouse" };
                var baat6 = new Baat { Navn = "HMK Pride" };
                var baat7 = new Baat { Navn = "Boaty McBoatface" };
                var baat8 = new Baat { Navn = "The Queen" };


                var ferd1 = new Ferd { Baat = baat1, Rute = rute1, AvreiseTid = new DateTime(2021, 12, 1, 13, 0, 0) , AnkomstTid = new DateTime(2021, 12, 2, 11, 0, 0)};

                //avreisedatoer for rute1
                for (int i = 1; i < 31; i += 1)
                {
                    if (i % 2 == 0)
                    {
                        context.Ferder.Add(new Ferd { Baat = baat1, Rute = rute1, AvreiseTid = new DateTime(2021, 12, i, 13, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 18, 0, 0) });
                    }
                    else
                    {
                        context.Ferder.Add(new Ferd { Baat = baat2, Rute = rute1, AvreiseTid = new DateTime(2021, 12, i, 8, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 16, 0, 0) });
                    }
                }

                //avreisedatoer for rute2
                for (int i = 1; i < 31; i++)
                {
                    if (i % 2 == 0)
                    {
                        context.Ferder.Add(new Ferd { Baat = baat3, Rute = rute2, AvreiseTid = new DateTime(2021, 12, i, 11, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 12, 0, 0) });
                    }
                    else
                    {
                        context.Ferder.Add(new Ferd { Baat = baat4, Rute = rute2, AvreiseTid = new DateTime(2021, 12, i, 10, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 1, 15, 0, 0) });
                    }

                }

                //avreisedatoer for rute 3
                for (int i = 1; i < 30; i++)
                {
                    if (i % 2 == 0)
                    {
                        context.Ferder.Add(new Ferd { Baat = baat5, Rute = rute3, AvreiseTid = new DateTime(2021, 12, i, 6, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 12, 0, 0) });
                    }
                    else
                    {
                        context.Ferder.Add(new Ferd { Baat = baat6, Rute = rute3, AvreiseTid = new DateTime(2021, 12, i, 8, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 14, 0, 0) });
                    }
                }

                //avreisedatoer for rute 4
                for (int i = 1; i < 30; i++)
                {
                    if (i % 2 == 0)
                    {
                        context.Ferder.Add(new Ferd { Baat = baat7, Rute = rute4, AvreiseTid = new DateTime(2021, 12, i, 7, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 17, 0, 0) });
                    }
                    else
                    {
                        context.Ferder.Add(new Ferd { Baat = baat8, Rute = rute4, AvreiseTid = new DateTime(2021, 12, i, 9, 0, 0), AnkomstTid = new DateTime(2021, 12, i + 2, 21, 0, 0) });
                    }
                }

                //Kun bestilling og billett til ferd1 foreløpig, kun 1 bestilling men 2 ruter, 2 båter og to mulige ferder

                var billett1 = new Billett { Ferd = ferd1, Voksen = true };
                var billett2 = new Billett { Ferd = ferd1, Voksen = false };

                List<Billett> billetter = new List<Billett>();
                billetter.Add(billett1);
                billetter.Add(billett2);

                //Hardkodet totalpris
                var bestilling1 = new Bestillinger { Billetter = billetter, TotalPris = 798 };
                context.Bestillinger.Add(bestilling1);
                context.SaveChanges();
            }
        }
    }
}

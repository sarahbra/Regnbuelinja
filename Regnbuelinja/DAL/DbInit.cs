using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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


                var båt1 = new Båt { Navn = "Båtten Anna" };
                var båt2 = new Båt { Navn = "Unicorn" };


                var ferd1 = new Ferd { Båt = båt1, Rute = rute1, AvreiseTid = new DateTime(2021, 12, 1, 13, 0, 0) , AnkomstTid = new DateTime(2021, 12, 2, 11, 0, 0)};

                //avreisedatoer for rute1
                for (int i = 2; i < 31; i+=2)
                {
                    context.Ferder.Add(new Ferd { Båt = båt1, Rute = rute1, AvreiseTid = new DateTime(2021, 12, i, 0, 0, 0), AnkomstTid = new DateTime(2021, 12, i+1, 0, 0, 0) });
                }

                //avreisedatoer for rute2
                for (int i = 1; i < 31; i++)
                {
                    context.Ferder.Add(new Ferd { Båt = båt1, Rute = rute2, AvreiseTid = new DateTime(2021, 12, i, 0, 0, 0), AnkomstTid = new DateTime(2021, 12, i+1, 0, 0, 0) });
                }

                //avreisedatoer for rute 3
                for (int i = 1; i < 30; i++)
                {
                    
                    context.Ferder.Add(new Ferd { Båt = båt2, Rute = rute3, AvreiseTid = new DateTime(2021, 12, i, 0, 0, 0), AnkomstTid = new DateTime(2021, 12, i+2, 0, 0, 0)});
                }

                //avreisedatoer for rute 4
                for (int i = 1; i < 30; i++)
                {
                    context.Ferder.Add(new Ferd { Båt = båt2, Rute = rute4, AvreiseTid = new DateTime(2021, 12, i, 0, 0, 0), AnkomstTid = new DateTime(2021, 12, i+2, 0, 0, 0)});
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

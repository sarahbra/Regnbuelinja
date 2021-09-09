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

                var rute1 = new Rute { Startpunkt = "Oslo", Endepunkt = "København", Pris = 399 };
                var rute2 = new Rute { Startpunkt = "København", Endepunkt = "Oslo", Pris = 399 };

                var båt1 = new Båt { Navn = "Båtten Anna"};
                var båt2 = new Båt { Navn = "Unicorn" };

                var ferd1 = new Ferd { Båt = båt1, Rute = rute1, Dato = "20.06.2022" };
                var ferd2 = new Ferd { Båt = båt2, Rute = rute2, Dato = "22.06.2022" };

                //Kun bestilling og billett til ferd1 foreløpig, kun 1 bestilling men 2 ruter, 2 båter og to mulige ferder

                var billett1 = new Billett { Ferd = ferd1, Voksen = true };
                var billett2 = new Billett { Ferd = ferd1, Voksen = false };

                List<Billett> billetter = new List<Billett>();
                billetter.Add(billett1);
                billetter.Add(billett2);

                //Hardkodet totalpris
                var bestillinger1 = new Bestillinger { Billetter = billetter, TotalPris= 798};

                context.SaveChanges();
            }

        }
    }
}

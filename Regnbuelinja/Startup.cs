using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Regnbuelinja.DAL;

namespace Regnbuelinja
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<BestillingContext>(options => options.UseSqlite("Data source=Bestilling.db"));
            services.AddScoped<IBestillingRepository, BestillingRepository>();
            services.AddDistributedMemoryCache();
            services.AddSession(options => {
                //Kan være inaktiv i 20 min
                options.IdleTimeout = System.TimeSpan.FromMinutes(20);
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddFile("Logs/logs.txt");
                app.UseDeveloperExceptionPage();
                DbInit.Initialize(app);
            }

            app.UseRouting();

            app.UseSession();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

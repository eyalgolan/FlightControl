using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FlightControlWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightControlWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<FlightContext>(opt =>
                opt.UseInMemoryDatabase("FlightList"));
            services.AddSingleton<IFlightManager, FlightManager>();
            services.AddSingleton<IFlightPlanManager, FlightPlanManager>();
            services.AddHttpClient("api", client =>
                client.DefaultRequestHeaders.Add("Accept", "application/json"));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new List<string> { "FlightsPage.html" } });
        }
    }
}

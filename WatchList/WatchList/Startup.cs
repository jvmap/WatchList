using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WatchList.Commands;
using WatchList.Config;
using WatchList.Data;
using WatchList.Data.SqlEventStore;
using WatchList.DynamicDispatch;
using WatchList.Events;
using WatchList.Services;

namespace WatchList
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
            services.AddRazorPages();
            services.AddScoped<IMovieRepository, OmdbMovieRepository>();
            services.AddSingleton<IUserMovieRepository, InMemoryUserMovieRepository>();
            services.AddSingleton<IEventPersistence, SqlEventPersistence>();
            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddSingleton<IClock, SystemClock>();
            services.AddScoped<CommandInvoker>();
            services.AddSingleton<DynamicDispatcher>();
            services.Configure<OmdbApiConfig>(this.Configuration.GetSection("OmdbApi"));
            services.AddHostedService<EventRoutingService>();
            services.AddDbContext<SqlEventPersistenceDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("EventStore")), ServiceLifetime.Singleton);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}

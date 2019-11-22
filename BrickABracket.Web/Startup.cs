using Autofac;
using BrickABracket.Core;
using BrickABracket.Core.ORM;
using BrickABracket.Derby;
using BrickABracket.FileProcessing;
using BrickABracket.Models;
using BrickABracket.NXT;
using BrickABracket.RoundRobin;
using BrickABracket.SingleElimination;
using BrickABracket.SwissSystem;
using BrickABracket.Web.Hubs;
using BrickABracket.Web.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BrickABracket.Web
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
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddSignalR();
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage()
                );
            services.AddHangfireServer();
            services.AddDbContext<BabContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("Context")));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register things directly with Autofac here
            builder.RegisterModule(new ModelsModule());
            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new FileProcessingModule());
            builder.RegisterModule(new NxtModule());
            builder.RegisterModule(new DerbyModule());
            builder.RegisterModule(new RoundRobinModule());
            builder.RegisterModule(new SingleEliminationModule());
            builder.RegisterModule(new SwissSystemModule());

            // Background Services
            builder.RegisterType<MatchWatcher>()
                .As<IHostedService>().SingleInstance();
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

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".webmanifest"] = "application/manifest+json";

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions{
                ContentTypeProvider = provider
            });
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseHangfireDashboard("/jobs");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapHub<TournamentHub>("/tournamentHub");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}

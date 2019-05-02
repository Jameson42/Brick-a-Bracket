using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using BrickABracket.Core;
using BrickABracket.Hubs;
using BrickABracket.Models;
using BrickABracket.Services;
using BrickABracket.NXT;
using BrickABracket.Derby;
using BrickABracket.RoundRobin;
using BrickABracket.SingleElimination;
using BrickABracket.SwissSystem;

namespace BrickABracket
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddSignalR(o => {
                o.EnableDetailedErrors = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseSpaStaticFiles();

            app.UseSignalR(routes =>{
                routes.MapHub<TournamentHub>("/tournamentHub");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
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

        // Use this to register things with Autofac
        // This is called after ConfigureServices, so will override registrations there
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ModelsModule());
            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new NxtModule());
            builder.RegisterModule(new DerbyModule());
            builder.RegisterModule(new RoundRobinModule());
            builder.RegisterModule(new SingleEliminationModule());
            builder.RegisterModule(new SwissSystemModule());
            builder.RegisterType<MatchWatcher>().SingleInstance();
        }
    }
}

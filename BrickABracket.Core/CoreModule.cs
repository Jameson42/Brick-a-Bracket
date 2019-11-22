using Autofac;
using LiteDB;
using BrickABracket.Core.Services;
using BrickABracket.Core.ORM;
using BrickABracket.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace BrickABracket.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
             // LiteDB Entities
            builder.RegisterInstance(
                new ConnectionString("Filename=BrickABracket.db;Mode=Exclusive")
            );
            builder.RegisterType<LiteRepository>().SingleInstance();

            // Services
            builder.RegisterType<ScoreTracker>().SingleInstance();
            builder.RegisterType<StatusTracker>().SingleInstance();
            builder.RegisterType<DeviceService>().SingleInstance()
                .AsSelf().AsImplementedInterfaces();
            builder.RegisterType<TournamentRunner>().SingleInstance();

            // Repositories
            builder.RegisterType<BabContext>()
                .As<DbContext>();
            builder.RegisterGeneric(typeof(Repository<>));
            builder.RegisterType<TournamentRepository>()
                .As<Repository<Tournament>>();
        }
    }
}
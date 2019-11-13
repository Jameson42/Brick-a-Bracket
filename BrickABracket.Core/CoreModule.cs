using Autofac;
using LiteDB;
using BrickABracket.Core.Services;

namespace BrickABracket.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // LiteDB Entities
            builder.RegisterInstance(new ConnectionString("BrickABracket.db"));
            builder.RegisterType<LiteRepository>();

            // Services
            builder.RegisterType<ScoreTracker>().SingleInstance();
            builder.RegisterType<StatusTracker>().SingleInstance();
            builder.RegisterType<DeviceService>().SingleInstance()
                .AsSelf().AsImplementedInterfaces();
            builder.RegisterType<TournamentRunner>().SingleInstance();
            builder.RegisterType<TournamentService>();
            builder.RegisterType<CompetitorService>();
            builder.RegisterType<MocService>();
            builder.RegisterType<MocImageService>();
            builder.RegisterType<ClassificationService>();
        }
    }
}
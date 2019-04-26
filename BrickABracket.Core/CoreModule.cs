using Autofac;
using LiteDB;
using BrickABracket.Core.Services;
using BrickABracket.Models.Base;

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
            builder.RegisterType<Tracker>().SingleInstance();
            builder.RegisterType<DeviceService>().SingleInstance();
            builder.RegisterType<ActivesService>().SingleInstance();
            builder.RegisterType<TournamentService>();
            builder.RegisterType<CompetitorService>();
            builder.RegisterType<MocService>();
            builder.RegisterType<ClassificationService>();
        }
    }
}
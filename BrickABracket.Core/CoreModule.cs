using Autofac;
using LiteDB;
using BrickABracket.Core.ORM;
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
            BsonMapper.Global.Entity<Moc>()
                .DbRef(x => x.Classification)
                .DbRef(x => x.Competitor);
            
            // Services
            builder.RegisterType<Tracker>().SingleInstance();
            builder.RegisterType<DeviceManager>().SingleInstance();
            builder.RegisterType<TournamentManager>();
            builder.RegisterType<CompetitorManager>();
            // TODO: Add managers for MOCs, Competitors, Classifications?
        }
    }
}
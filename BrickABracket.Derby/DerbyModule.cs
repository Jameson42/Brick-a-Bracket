using Autofac;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Derby
{
    public class DerbyModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DerbyTournament>()
                .As<ITournament>()
                .WithMetadata("Type", "derby");
        }
    }
}
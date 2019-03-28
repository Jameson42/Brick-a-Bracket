using Autofac;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.SwissSystem
{
    public class SwissSystemModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SwissSystemTournament>()
                .As<ITournament>()
                .WithMetadata("Type", "swisssystem");
        }
    }
}
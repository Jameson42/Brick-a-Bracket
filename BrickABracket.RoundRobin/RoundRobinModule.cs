using Autofac;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.RoundRobin
{
    public class RoundRobinModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RoundRobinTournament>()
                .As<ITournamentStrategy>()
                .WithMetadata("Type", "roundrobin");
        }
    }
}
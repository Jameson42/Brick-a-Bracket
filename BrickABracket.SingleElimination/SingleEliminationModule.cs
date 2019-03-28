using Autofac;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.SingleElimination
{
    public class SingleEliminationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SingleEliminationTournament>()
                .As<ITournament>()
                .WithMetadata("Type", "singleelimination");
        }
    }
}
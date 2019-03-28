using Autofac;
using BrickABracket.Core.ORM;

namespace BrickABracket.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Repository>()
                .WithParameter("connectionString","BrickABracket.db")
                .InstancePerLifetimeScope();
            builder.RegisterType<Tracker>().SingleInstance();
        }
    }
}
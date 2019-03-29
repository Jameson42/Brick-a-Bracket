using Autofac;
using BrickABracket.Models.Base;

namespace BrickABracket.Models
{
    public class ModelsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Category>();
            builder.RegisterType<Classification>();
            builder.RegisterType<Match>();
            builder.RegisterType<Moc>();
            builder.RegisterType<Round>();
            builder.RegisterType<Score>();
            builder.RegisterType<Tournament>();
        }
    }
}

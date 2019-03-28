using Autofac;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models
{
    public class ModelsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Category>().As<ICategory>();
            builder.RegisterType<Classification>().As<IClassification>();
            builder.RegisterType<Match>().As<IMatch>();
            builder.RegisterType<Moc>().As<IMoc>();
            builder.RegisterType<Round>().As<IRound>();
            builder.RegisterType<Score>().As<IScore>();
        }
    }
}

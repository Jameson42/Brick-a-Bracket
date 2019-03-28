using Autofac;
using BrickABracket.Models.Base;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.Models
{
    public class ModelModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Add builder.Register calls
            builder.RegisterType(typeof(Classification)).AsImplementedInterfaces();
            builder.RegisterType(typeof(Score)).AsImplementedInterfaces();
        }
    }
}

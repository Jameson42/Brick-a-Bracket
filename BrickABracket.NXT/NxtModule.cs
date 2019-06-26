using Autofac;
using BrickABracket.Models.Interfaces;

namespace BrickABracket.NXT
{
    public class NxtModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Nxt>()
                .As<IDevice>()
                .WithMetadata("Type", "NXT")
                .WithMetadata("BluetoothPorts", Nxt.BluetoothPorts);
        }
    }
}
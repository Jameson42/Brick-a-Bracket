using System;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BrickABracket.Core;
using BrickABracket.Models;
using BrickABracket.Models.Interfaces;
using BrickABracket.NXT;
using BrickABracket.Derby;
using BrickABracket.RoundRobin;
using BrickABracket.SingleElimination;
using BrickABracket.SwissSystem;

namespace BrickABracket.Console
{
    public class Program
    {
        private static IServiceProvider _serviceProvider;
        private static IContainer _container;

        public static void Main()
        {
            RegisterServices();
            
            using(var scope = _container.BeginLifetimeScope())
            {
                var nxt = scope.Resolve<IDevice>(new NamedParameter("connectionString","loopback"));
                System.Console.WriteLine(nxt.Connected);
            }

            System.Console.WriteLine("Hello World!");
            DisposeServices();
        }

        private static void RegisterServices()
        {
            var serviceCollection = new ServiceCollection();
            var builder = new ContainerBuilder();
            builder.RegisterModule(new ModelsModule());
            builder.RegisterModule(new CoreModule());
            builder.RegisterModule(new NxtModule());
            builder.RegisterModule(new DerbyModule());
            builder.RegisterModule(new RoundRobinModule());
            builder.RegisterModule(new SingleEliminationModule());
            builder.RegisterModule(new SwissSystemModule());

            builder.Populate(serviceCollection);
            _container = builder.Build();
            _serviceProvider = new AutofacServiceProvider(_container);
        }

        private static void DisposeServices()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}

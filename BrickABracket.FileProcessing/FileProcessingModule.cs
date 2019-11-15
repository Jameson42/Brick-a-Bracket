using Autofac;

namespace BrickABracket.FileProcessing
{
    public class FileProcessingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Services
            builder.RegisterType<MocImageService>();
            builder.RegisterType<CompetitorImportService>();
        }
    }
}
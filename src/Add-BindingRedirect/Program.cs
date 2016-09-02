using System.IO;
using Add_BindingRedirect.BindingRedirection;
using Add_BindingRedirect.CsprojReading;
using Add_BindingRedirect.DirectoryScanning;
using Serilog;
using Yaclp;

namespace Add_BindingRedirect
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var parameters = DefaultParser.ParseOrExitWithUsageMessage<Parameters>(args);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .MinimumLevel.Is(parameters.LogLevel)
                .MinimumLevel.Verbose()
                .CreateLogger();

            var currentDirectory = new DirectoryInfo(parameters.Directory);

            var bindingRedirector = new BindingRedirector(Log.Logger, new RecursingDirectoryScanner(), new CsprojReferenceScanner(), new AssemblyReferenceConsolidator());
            bindingRedirector.AddBindingRedirects(currentDirectory);
        }
    }
}
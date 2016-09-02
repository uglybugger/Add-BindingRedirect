using System.IO;
using System.Linq;
using Add_BindingRedirect.BindingRedirection;
using Add_BindingRedirect.CsprojReading;
using Add_BindingRedirect.DirectoryScanning;
using Serilog;

namespace Add_BindingRedirect
{
    public class BindingRedirector
    {
        private readonly AssemblyReferenceConsolidator _assemblyReferenceConsolidator;
        private readonly CsprojReferenceScanner _csprojReferenceScanner;
        private readonly ILogger _logger;
        private readonly RecursingDirectoryScanner _recursingDirectoryScanner;

        public BindingRedirector(ILogger logger,
            RecursingDirectoryScanner recursingDirectoryScanner,
            CsprojReferenceScanner csprojReferenceScanner,
            AssemblyReferenceConsolidator assemblyReferenceConsolidator)
        {
            _logger = logger;
            _recursingDirectoryScanner = recursingDirectoryScanner;
            _csprojReferenceScanner = csprojReferenceScanner;
            _assemblyReferenceConsolidator = assemblyReferenceConsolidator;
        }

        public void AddBindingRedirects(DirectoryInfo currentDirectory)
        {
            _logger.Verbose("Scanning {CurrentDirectory} and all subdirectories for .csproj files", currentDirectory.FullName);

            var csprojFiles = _recursingDirectoryScanner
                .Scan(currentDirectory, f => f.Extension == ".csproj")
                .ToArray();

            var assemblyReferences = csprojFiles
                .SelectMany(csproj => _csprojReferenceScanner.ScanForReferencedAssemblies(csproj))
                .ToArray();

            var toConsolidate = _assemblyReferenceConsolidator.DeriveAssembliesToConsolidate(assemblyReferences).ToArray();

            foreach (var csproj in csprojFiles)
            {
                var bindingRedirector = new SingleProjectBindingRedirector(_csprojReferenceScanner, new BindingRedirectorXmlEditor());
                bindingRedirector.Redirect(csproj, toConsolidate);
            }
        }
    }
}
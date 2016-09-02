using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Add_BindingRedirect.CsprojReading;
using Serilog;
using ThirdDrawer.Extensions.CollectionExtensionMethods;

namespace Add_BindingRedirect.BindingRedirection
{
    internal class SingleProjectBindingRedirector
    {
        private readonly BindingRedirectorXmlEditor _bindingRedirectorXmlEditor;
        private readonly CsprojReferenceScanner _csprojReferenceScanner;

        public SingleProjectBindingRedirector(CsprojReferenceScanner csprojReferenceScanner, BindingRedirectorXmlEditor bindingRedirectorXmlEditor)
        {
            _csprojReferenceScanner = csprojReferenceScanner;
            _bindingRedirectorXmlEditor = bindingRedirectorXmlEditor;
        }

        public void Redirect(FileInfo csproj, IGrouping<string, AssemblyName>[] globalReferencesToConsolidate)
        {
            var csprojReferencesToConsolidate = FindReferencesToConsolidate(csproj, globalReferencesToConsolidate).ToArray();
            if (csprojReferencesToConsolidate.None())
            {
                Log.Verbose("{Csproj} does not have any assembly references requiring redirection", csproj.Name);
                return;
            }

            var configFile = LocateConfigFile(csproj);

            if (configFile == null)
            {
                Log.Warning("Corresponding [app|web] config does not exist for {Csproj}. Not adding redirects.", csproj.Name);
                return;
            }

            XDocument xml;
            using (var fs = configFile.OpenRead())
            {
                xml = XDocument.Load(fs);
            }

            _bindingRedirectorXmlEditor.AddOrUpdateBindingRedirects(xml, csprojReferencesToConsolidate);

            var writerSettings = new XmlWriterSettings
            {
                NewLineChars = Environment.NewLine,
                Indent = true
            };
            using (var writer = XmlWriter.Create(configFile.FullName, writerSettings))
            {
                xml.WriteTo(writer);
            }
        }

        private static FileInfo LocateConfigFile(FileInfo csproj)
        {
            var configFileNames = new[] {"web.config", "app.config"}.Select(x => x.ToUpperInvariant()).ToArray();
            var configFile = csproj.Directory.GetFiles()
                .Where(fn => configFileNames.Contains(fn.Name.ToUpperInvariant()))
                .FirstOrDefault();
            return configFile;
        }

        private IEnumerable<AssemblyName> FindReferencesToConsolidate(FileInfo csproj, IGrouping<string, AssemblyName>[] globalReferencesToConsolidate)
        {
            var references = _csprojReferenceScanner.ScanForReferencedAssemblies(csproj).ToArray();

            foreach (var reference in references)
            {
                var match = globalReferencesToConsolidate.Where(g => g.Key == reference.Name).FirstOrDefault();
                if (match == null)
                {
                    Log.Verbose("{AssemblyName} referenced in {Csproj} does not require redirection", reference.Name, csproj.Name);
                    continue;
                }

                var highestVersionedAssembltName = match.OrderByDescending(an => an.Version).First();
                Log.Debug("Redirecting {AssemblyName} referenced in {Csproj} to {AssemblyVersion}", reference.Name, csproj.Name, highestVersionedAssembltName.Version.ToString());
                yield return highestVersionedAssembltName;
            }
        }
    }
}
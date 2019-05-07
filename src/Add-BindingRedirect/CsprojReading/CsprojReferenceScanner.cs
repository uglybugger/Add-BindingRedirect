using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Add_BindingRedirect.CsprojReading
{
    public class CsprojReferenceScanner
    {
        private Regex excludeAssemblyRegex;
        private bool excludeEnabled = false;

        public CsprojReferenceScanner(string excludeAssemblyRegex)
        {
            if (!string.IsNullOrWhiteSpace(excludeAssemblyRegex))
            {
                this.excludeAssemblyRegex = new Regex(excludeAssemblyRegex);
                this.excludeEnabled = true;
            }
        }

        public IEnumerable<AssemblyName> ScanForReferencedAssemblies(FileInfo csproj)
        {
            XDocument xml;
            using (var fs = csproj.OpenRead())
            {
                xml = XDocument.Load(fs);
            }

            var references = xml
                .Descendants()
                .Where(xe => xe.Name.LocalName == "Reference")
                .ToArray();
            var assemblyNames = references
                .Select(r => r.Attribute("Include").Value)
                .Where(r => !(excludeEnabled && excludeAssemblyRegex.IsMatch(r)))
                .Select(n => new AssemblyName(n))
                .ToArray();

            return assemblyNames;
        }
    }
}
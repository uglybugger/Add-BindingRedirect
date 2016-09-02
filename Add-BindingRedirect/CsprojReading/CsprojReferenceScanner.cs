using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Add_BindingRedirect.CsprojReading
{
    public class CsprojReferenceScanner
    {
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
                .Select(n => new AssemblyName(n))
                .ToArray();

            return assemblyNames;
        }
    }
}
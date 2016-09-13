using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Add_BindingRedirect.BindingRedirection
{
    public class AssemblyBindingNodeComparer : IComparer<XElement>
    {
        private readonly XmlNamespaceManager _nsm;

        public AssemblyBindingNodeComparer()
        {
            _nsm = new XmlNamespaceManager(new NameTable());
            _nsm.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1");
        }

        public int Compare(XElement x, XElement y)
        {
            var xDependentAssemblyNode = x.XPathSelectElements("asm:assemblyIdentity", _nsm).SingleOrDefault();
            var yDependentAssemblyNode = y.XPathSelectElements("asm:assemblyIdentity", _nsm).SingleOrDefault();

            var xAssemblyName = xDependentAssemblyNode?.Attribute("name").Value;
            var yAssemblyName = yDependentAssemblyNode?.Attribute("name").Value;

            return string.CompareOrdinal(xAssemblyName, yAssemblyName);
        }
    }
}
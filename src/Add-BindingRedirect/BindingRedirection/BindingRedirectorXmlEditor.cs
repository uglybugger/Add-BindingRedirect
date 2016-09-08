using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Add_BindingRedirect.Extensions;
using ThirdDrawer.Extensions.CollectionExtensionMethods;

namespace Add_BindingRedirect.BindingRedirection
{
    public class BindingRedirectorXmlEditor
    {
        public void AddOrUpdateBindingRedirects(XDocument xml, AssemblyName[] csprojReferencesToConsolidate)
        {
            var nsm = new XmlNamespaceManager(new NameTable());
            nsm.AddNamespace("asm", "urn:schemas-microsoft-com:asm.v1");

            var configurationNode = xml.GetOrCreateChild("configuration");
            var runtimeNode = configurationNode.GetOrCreateChild("runtime");
            var assemblyBindingNode = runtimeNode.GetOrCreateChild("assemblyBinding", "urn:schemas-microsoft-com:asm.v1");

            foreach (var refToConsolidate in csprojReferencesToConsolidate)
            {
                var existingNodes = assemblyBindingNode.XPathSelectElements($"asm:dependentAssembly/asm:assemblyIdentity[@name='{refToConsolidate.Name}']", nsm).ToArray();
                existingNodes
                    .Do(n => n.Parent.Remove())
                    .Done();

                var publicKeyTokenBytes = refToConsolidate.GetPublicKeyToken();
                var publicKeyToken = publicKeyTokenBytes != null
                                         ? BitConverter.ToString(publicKeyTokenBytes).Replace("-", string.Empty).ToLowerInvariant()
                                         : "null";
                var cultureName = string.IsNullOrWhiteSpace(refToConsolidate.CultureName) ? "neutral" : refToConsolidate.CultureName;
                var s =
                    $@"
      <dependentAssembly xmlns=""urn:schemas-microsoft-com:asm.v1"">
        <assemblyIdentity name=""{refToConsolidate.Name}"" publicKeyToken=""{publicKeyToken}"" culture=""{cultureName}""/>
        <bindingRedirect oldVersion=""0.0.0.0-{refToConsolidate.Version}"" newVersion=""{refToConsolidate.Version}""/>
      </dependentAssembly>
";
                var element = XElement.Parse(s);
                assemblyBindingNode.Add(element);
            }

            assemblyBindingNode.SortChildren(new AssemblyBindingNodeComparer());
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Add_BindingRedirect.Extensions
{
    internal static class XElementExtensions
    {
        public static XElement GetOrCreateChild(this XContainer parentNode, string childName, string childNamespace = null)
        {
            var name = childNamespace != null
                ? XName.Get(childName, childNamespace)
                : XName.Get(childName);
            var childNode = parentNode.Element(name);
            if (childNode == null)
            {
                childNode = new XElement(name);
                parentNode.Add(childNode);
            }
            return childNode;
        }

        public static void SortChildren(this XContainer parent, IComparer<XElement> comparer)
        {
            var children = parent.Elements().ToArray();
            foreach (var child in children) child.Remove();

            var sortedChildren = children
                .OrderBy(c => c, comparer)
                .ToArray();

            foreach (var child in sortedChildren) parent.Add(child);
        }
    }
}
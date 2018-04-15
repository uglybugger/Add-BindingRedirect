using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serilog;
using ThirdDrawer.Extensions.CollectionExtensionMethods;

namespace Add_BindingRedirect.BindingRedirection
{
    public class AssemblyReferenceConsolidator
    {
        public IEnumerable<IGrouping<string, AssemblyName>> DeriveAssembliesToConsolidate(AssemblyName[] assemblyReferences)
        {
            Log.Verbose("Consolidating assemblies that have different referenced versions");

            var groupings = assemblyReferences
                .NotNull()
                .GroupBy(an => an.Name)
                .OrderBy(g => g.Key);

            foreach (var g in groupings)
            {
                var distinctAssemblyNames = g
                    .GroupBy(an => an.Version)
                    .Select(x => x.First())
                    .Where(an => an.Version != null)
                    .ToArray();

                var distinctVersions = distinctAssemblyNames.Select(an => an.Version.ToString()).ToArray();

                var assemblyName = g.Key;
                if (distinctAssemblyNames.Length == 0)
                {
                    Log.Verbose("There is no version information specified for {AssemblyName}.", assemblyName);
                    continue;
                }

                Log.Debug("There are {VersionCount} distinct version(s) of {AssemblyName}: {@DistinctVersions}", distinctAssemblyNames.Length, assemblyName, distinctVersions);
                yield return g;
            }
        }
    }
}
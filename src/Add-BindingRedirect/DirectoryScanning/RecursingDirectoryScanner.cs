using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ThirdDrawer.Extensions.CollectionExtensionMethods;

namespace Add_BindingRedirect.DirectoryScanning
{
    public class RecursingDirectoryScanner
    {
        public IEnumerable<FileInfo> Scan(DirectoryInfo startingDirectory, Func<FileInfo, bool> isMatch)
        {
            var allDescendantDirectories = new[] {startingDirectory}.DepthFirst(d => d.GetDirectories()).ToArray();

            var matchingFiles = allDescendantDirectories
                .SelectMany(d => d.GetFiles())
                .Where(isMatch)
                .ToArray();

            return matchingFiles;
        }
    }
}
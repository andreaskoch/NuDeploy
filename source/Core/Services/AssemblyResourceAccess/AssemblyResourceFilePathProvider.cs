using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public class AssemblyResourceFilePathProvider : IAssemblyResourceFilePathProvider
    {
        private const char NamespaceSeperator = '.';

        private const string PathSeperator = @"\";

        public string GetRelativeFilePath(string baseNamespace, string assemblyResourceName)
        {
            if (string.IsNullOrWhiteSpace(baseNamespace))
            {
                throw new ArgumentException("baseNamespace");
            }

            if (string.IsNullOrWhiteSpace(assemblyResourceName))
            {
                throw new ArgumentException("assemblyResourceName");
            }

            // remove white space
            baseNamespace = baseNamespace.Trim();

            // ensure namespace ends with a trailing .
            baseNamespace = baseNamespace.TrimEnd(NamespaceSeperator);
            baseNamespace += NamespaceSeperator.ToString(CultureInfo.InvariantCulture);

            if (!assemblyResourceName.StartsWith(baseNamespace))
            {
                // the supplied assembly ressource is not a member of the supplied base namespace
                throw new ArgumentException("baseNamespace");
            }

            var subNamespace = assemblyResourceName.Substring(baseNamespace.Length);
            var fileNameFragments = subNamespace.Split(NamespaceSeperator);

            string relativeFilePath = Path.Combine(
                string.Join(PathSeperator, fileNameFragments.Take(fileNameFragments.Length - 2)),
                string.Join(NamespaceSeperator.ToString(CultureInfo.InvariantCulture), fileNameFragments.Skip(fileNameFragments.Length - 2).Take(2)));

            return relativeFilePath;
        }
    }
}
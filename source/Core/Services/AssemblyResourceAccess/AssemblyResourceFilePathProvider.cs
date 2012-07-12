using System.IO;
using System.Linq;

namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public class AssemblyResourceFilePathProvider : IAssemblyResourceFilePathProvider
    {
        public string GetRelativeFilePath(string baseNamespace, string assemblyResourceName)
        {
            var fileNameFragments = assemblyResourceName.Replace(baseNamespace, string.Empty).Split('.');
            string relativeFilePath = Path.Combine(
                string.Join(@"\", fileNameFragments.Take(fileNameFragments.Length - 2)),
                string.Join(".", fileNameFragments.Skip(fileNameFragments.Length - 2).Take(2)));

            return relativeFilePath;
        }
    }
}
using System;
using System.IO;
using System.Text;

using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Tests
{
    public class TestUtilities
    {
        public static StreamReader GetStreamReaderForText(string text)
        {
            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(text)));
        }

        public static NuDeployPackageInfo GetPackage(string id, bool isInstalled, int revision = 0)
        {
            var version = new SemanticVersion(1, 0, 0, revision);
            string folderName = string.Format("{0}.{1}", id, version);
            string folderPath = Path.Combine(Environment.CurrentDirectory, folderName);

            return new NuDeployPackageInfo { Id = id, IsInstalled = isInstalled, Folder = folderPath, Version = version };
        }
    }
}
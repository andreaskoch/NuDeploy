using System;
using System.Linq;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services.Filesystem
{
    public class RelativeFilePathInfoFactory : IRelativeFilePathInfoFactory
    {
        public RelativeFilePathInfo GetRelativeFilePathInfo(string absoluteFilePath, string basePath)
        {
            if (string.IsNullOrWhiteSpace(absoluteFilePath))
            {
                throw new ArgumentException("absoluteFilePath");
            }

            if (string.IsNullOrWhiteSpace(basePath))
            {
                throw new ArgumentException("basePath");
            }

            if (absoluteFilePath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
            {
                string relativePath = absoluteFilePath.Substring(basePath.Length).TrimStart(NuDeployConstants.PathSeperator.ToCharArray().First());
                return new RelativeFilePathInfo(absoluteFilePath, relativePath);
            }

            return null;
        }
    }
}
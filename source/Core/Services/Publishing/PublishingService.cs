using System;
using System.IO;

using NuDeploy.Core.Common.FilesystemAccess;

namespace NuDeploy.Core.Services.Publishing
{
    public class PublishingService : IPublishingService
    {
        public const int DefaultTimeoutInMinutes = 5;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IPackageServerFactory packageServerFactory;

        private readonly IPublishConfigurationAccessor publishConfigurationAccessor;

        private TimeSpan defaultTimeout = TimeSpan.FromMinutes(DefaultTimeoutInMinutes);

        public PublishingService(IFilesystemAccessor filesystemAccessor, IPackageServerFactory packageServerFactory, IPublishConfigurationAccessor publishConfigurationAccessor)
        {
            if (filesystemAccessor == null)
            {
                throw new ArgumentNullException("filesystemAccessor");
            }

            if (packageServerFactory == null)
            {
                throw new ArgumentNullException("packageServerFactory");
            }

            if (publishConfigurationAccessor == null)
            {
                throw new ArgumentNullException("publishConfigurationAccessor");
            }

            this.filesystemAccessor = filesystemAccessor;
            this.packageServerFactory = packageServerFactory;
            this.publishConfigurationAccessor = publishConfigurationAccessor;
        }

        public bool PublishPackage(string packagePath, string packageServerConfigurationName)
        {
            if (string.IsNullOrWhiteSpace(packagePath))
            {
                throw new ArgumentException("packagePath");
            }

            if (!this.filesystemAccessor.FileExists(packagePath))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(packageServerConfigurationName))
            {
                throw new ArgumentException("configurationName");
            }

            PublishConfiguration publishConfiguration = this.publishConfigurationAccessor.GetPublishConfiguration(packageServerConfigurationName);
            if (publishConfiguration == null || !publishConfiguration.IsValid)
            {
                return false;
            }

            var packageServer = this.packageServerFactory.GetPackageServer(publishConfiguration.PublishLocation);
            if (packageServer == null)
            {
                return false;
            }

            using (Stream packageStream = this.filesystemAccessor.GetReadStream(packagePath))
            {
                if (packageStream == null)
                {
                    return false;
                }

                try
                {
                    if (publishConfiguration.IsLocal)
                    {
                        var packageFile = new FileInfo(packagePath);
                        string targetPath = Path.Combine(publishConfiguration.PublishLocation, packageFile.Name);
                        using (Stream writeStream = this.filesystemAccessor.GetWriteStream(targetPath))
                        {
                            packageStream.CopyTo(writeStream);
                        }
                    }
                    else
                    {
                        packageServer.PushPackage(publishConfiguration.ApiKey, packageStream, Convert.ToInt32(this.defaultTimeout.TotalMilliseconds));
                    }
                    
                    return true;
                }
                catch (Exception publishException)
                {
                    return false;
                }
            }
        }
    }
}
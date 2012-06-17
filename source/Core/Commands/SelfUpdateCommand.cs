using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using NuDeploy.Core.Common;

using NuGet;

namespace NuDeploy.Core.Commands
{
    public class SelfUpdateCommand : ICommand
    {
        private const string CommandName = "selfupdate";

        private readonly string[] alternativeCommandNames = new[] { "update" };

        private readonly IUserInterface userInterface;

        private readonly ApplicationInformation applicationInformation;

        private readonly IPackageRepositoryFactory repositoryFactory;

        public SelfUpdateCommand(IUserInterface userInterface, ApplicationInformation applicationInformation, IPackageRepositoryFactory repositoryFactory)
        {
            this.userInterface = userInterface;
            this.applicationInformation = applicationInformation;
            this.repositoryFactory = repositoryFactory;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new string[] { },
                PositionalArguments = new string[] { },
                Description = Resources.SelfUpdateCommand.CommandDescriptionText,
                Usage = string.Format("{0}", CommandName),
                Examples = new Dictionary<string, string>
                    {
                        {
                            string.Format("{0}", CommandName),
                            Resources.SelfUpdateCommand.CommandExampleDescription1
                        }
                    },
                ArgumentDescriptions = new Dictionary<string, string>()
            };

            this.Arguments = new Dictionary<string, string>();
        }

        public CommandAttributes Attributes { get; private set; }

        public IDictionary<string, string> Arguments { get; set; }

        public void Execute()
        {
            Assembly assembly = this.GetType().Assembly;
            this.SelfUpdate(assembly.Location, new SemanticVersion(assembly.GetName().Version));
        }

        internal void SelfUpdate(string exePath, SemanticVersion version)
        {
            //Console.WriteLine(NuGetResources.UpdateCommandCheckingForUpdates, NuGetConstants.DefaultFeedUrl);

            // Get the nuget command line package from the specified repository
            IPackageRepository packageRepository = this.repositoryFactory.CreateRepository(NuDeployConstants.DefaultFeedUrl);

            IPackage package = packageRepository.FindPackage(NuDeployConstants.NuDeployCommandLinePackageId);

            // We didn't find it so complain
            if (package == null)
            {
                throw new Exception("NuGetResources.UpdateCommandUnableToFindPackage, NuGetCommandLinePackageId");
            }

            //Console.WriteLine(NuGetResources.UpdateCommandCurrentlyRunningNuGetExe, version);

            // Check to see if an update is needed
            if (version >= package.Version)
            {
                //Console.WriteLine(NuGetResources.UpdateCommandNuGetUpToDate);
            }
            else
            {
                //Console.WriteLine(NuGetResources.UpdateCommandUpdatingNuGet, package.Version);

                // Get NuGet.exe file from the package
                IPackageFile file =
                    package.GetFiles().FirstOrDefault(
                        f => Path.GetFileName(f.Path).Equals(this.applicationInformation.NameOfExecutable, StringComparison.OrdinalIgnoreCase));

                // If for some reason this package doesn't have NuGet.exe then we don't want to use it
                if (file == null)
                {
                    throw new Exception("NuGetResources.UpdateCommandUnableToLocateNuGetExe");
                }

                // Get the exe path and move it to a temp file (NuGet.exe.old) so we can replace the running exe with the bits we got 
                // from the package repository
                string renamedPath = exePath + ".old";
                Move(exePath, renamedPath);

                // Update the file
                UpdateFile(exePath, file);

                //Console.WriteLine(NuGetResources.UpdateCommandUpdateSuccessful);
            }
        }

        protected virtual void UpdateFile(string exePath, IPackageFile file)
        {
            using (Stream fromStream = file.GetStream(), toStream = File.Create(exePath))
            {
                fromStream.CopyTo(toStream);
            }
        }

        protected virtual void Move(string oldPath, string newPath)
        {
            try
            {
                if (File.Exists(newPath))
                {
                    File.Delete(newPath);
                }
            }
            catch (FileNotFoundException)
            {

            }

            File.Move(oldPath, newPath);
        }
    }

    public class PackageRepositoryFactory : IPackageRepositoryFactory
    {
        private static readonly PackageRepositoryFactory _default = new PackageRepositoryFactory();
        private static readonly Func<Uri, IHttpClient> _defaultHttpClientFactory = u => new RedirectedHttpClient(u);
        private Func<Uri, IHttpClient> _httpClientFactory;

        public static PackageRepositoryFactory Default
        {
            get
            {
                return _default;
            }
        }

        public Func<Uri, IHttpClient> HttpClientFactory
        {
            get { return _httpClientFactory ?? _defaultHttpClientFactory; }
            set { _httpClientFactory = value; }
        }

        public virtual IPackageRepository CreateRepository(string packageSource)
        {
            if (packageSource == null)
            {
                throw new ArgumentNullException("packageSource");
            }

            Uri uri = new Uri(packageSource);
            if (uri.IsFile)
            {
                return new LocalPackageRepository(uri.LocalPath);
            }

            var client = HttpClientFactory(uri);

            // Make sure we get resolve any fwlinks before creating the repository
            return new DataServicePackageRepository(client);
        }
    }

    public class CommandLineRepositoryFactory : PackageRepositoryFactory
    {
        public static readonly string UserAgent = "NuGet Command Line";

        public override IPackageRepository CreateRepository(string packageSource)
        {
            var repository = base.CreateRepository(packageSource);
            var httpClientEvents = repository as IHttpClientEvents;

            if (httpClientEvents != null)
            {
                httpClientEvents.SendingRequest += (sender, args) =>
                {
                    string userAgent = HttpUtility.CreateUserAgentString(UserAgent);
                    HttpUtility.SetUserAgent(args.Request, userAgent);
                };
            }

            return repository;
        }
    }
}
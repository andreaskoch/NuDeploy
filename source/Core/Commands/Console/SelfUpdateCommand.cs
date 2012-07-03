using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using NuDeploy.Core.Common;
using NuDeploy.Core.Services;

using NuGet;

namespace NuDeploy.Core.Commands.Console
{
    public class SelfUpdateCommand : ICommand
    {
        private const string CommandName = "selfupdate";

        private readonly string[] alternativeCommandNames = new[] { "update" };

        private readonly IUserInterface userInterface;

        private readonly ApplicationInformation applicationInformation;

        private readonly IPackageRepositoryBrowser packageRepositoryBrowser;

        public SelfUpdateCommand(IUserInterface userInterface, ApplicationInformation applicationInformation, IPackageRepositoryBrowser packageRepositoryBrowser)
        {
            this.userInterface = userInterface;
            this.applicationInformation = applicationInformation;
            this.packageRepositoryBrowser = packageRepositoryBrowser;

            this.Attributes = new CommandAttributes
            {
                CommandName = CommandName,
                AlternativeCommandNames = this.alternativeCommandNames,
                RequiredArguments = new string[] { },
                PositionalArguments = new string[] { },
                Description = string.Format(Resources.SelfUpdateCommand.CommandDescriptionTextTemplate, this.applicationInformation.NameOfExecutable),
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

        private void SelfUpdate(string exePath, SemanticVersion version)
        {
            string selfUpdateMessage = string.Format(
                Resources.SelfUpdateCommand.SelfupdateMessageTemplate, 
                NuDeployConstants.NuDeployCommandLinePackageId, 
                NuDeployConstants.DefaultFeedUrl);

            this.userInterface.WriteLine(selfUpdateMessage);

            // fetch package
            IPackageRepository packageRepository;
            IPackage package = this.packageRepositoryBrowser.FindPackage(NuDeployConstants.NuDeployCommandLinePackageId, out packageRepository);
            if (package == null)
            {
                this.userInterface.WriteLine(Resources.SelfUpdateCommand.PackageNotFound);
                return;
            }

            // version check
            this.userInterface.WriteLine(string.Format(Resources.SelfUpdateCommand.CurrentVersionTemplate, this.applicationInformation.NameOfExecutable, version));
            if (version >= package.Version)
            {
                this.userInterface.WriteLine(string.Format(Resources.SelfUpdateCommand.NoUpdateRequiredMessageTemplate, this.applicationInformation.NameOfExecutable));
                return;
            }

            // update
            this.userInterface.WriteLine(string.Format(Resources.SelfUpdateCommand.UpdateMessageTemplate, this.applicationInformation.NameOfExecutable, package.Version));

            IPackageFile executable =
                package.GetFiles().FirstOrDefault(
                    file => Path.GetFileName(file.Path).Equals(this.applicationInformation.NameOfExecutable, StringComparison.OrdinalIgnoreCase));

            if (executable == null)
            {
                this.userInterface.WriteLine(
                    string.Format(
                        Resources.SelfUpdateCommand.ExecutableNotFoundInPackageMessageTemplate,
                        NuDeployConstants.NuDeployCommandLinePackageId,
                        this.applicationInformation.NameOfExecutable));
            }

            // Get the exe path and move it to a temp file (NuGet.exe.old) so we can replace the running exe with the bits we got from the package repository
            string renamedPath = exePath + ".old";
            this.MoveFile(exePath, renamedPath);

            // Update the file
            this.UpdateFile(exePath, executable);

            this.userInterface.WriteLine(Resources.SelfUpdateCommand.UpdateSuccessful);
        }

        private void UpdateFile(string exePath, IPackageFile file)
        {
            using (Stream fromStream = file.GetStream(), toStream = File.Create(exePath))
            {
                fromStream.CopyTo(toStream);
            }
        }

        private void MoveFile(string oldPath, string newPath)
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
}
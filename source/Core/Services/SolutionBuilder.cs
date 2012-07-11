using System.Collections.Generic;
using System.IO;

using Microsoft.Build.Execution;

using NuDeploy.Core.Common;

namespace NuDeploy.Core.Services
{
    public class SolutionBuilder : ISolutionBuilder
    {
        private const string BuildPropertyNameTargetPlatform = "Platform";

        private const string BuildPropertyNameOutputPath = "OutputPath";

        private const string BuildPropertyNameBuildConfiguration = "Configuration";

        private const string DefaultBuildTarget = "Rebuild";

        private const string DefaultTargetPlatform = "Any CPU";

        private readonly ApplicationInformation applicationInformation;

        private readonly IFilesystemAccessor filesystemAccessor;

        private readonly IUserInterface userInterface;

        private readonly string buildFolder;

        public SolutionBuilder(ApplicationInformation applicationInformation, IFilesystemAccessor filesystemAccessor, IUserInterface userInterface)
        {
            this.applicationInformation = applicationInformation;
            this.filesystemAccessor = filesystemAccessor;
            this.userInterface = userInterface;
            this.buildFolder = this.GetBuildFolderPath();
        }

        public string BuildFolder
        {
            get
            {
                return this.buildFolder;
            }
        }

        public bool Build(string solutionPath, string buildConfiguration, IEnumerable<KeyValuePair<string, string>> buildProperties)
        {
            if (!this.PrepareBuildFolder())
            {
                this.userInterface.WriteLine(string.Format("Could not prepare the build folder \"{0}\". Please check the folder and try again.", this.buildFolder));
                return false;
            }

            // prepare build parameters
            var buildParameters = new Dictionary<string, string>
                {
                    { BuildPropertyNameBuildConfiguration, buildConfiguration },
                    { BuildPropertyNameTargetPlatform, DefaultTargetPlatform },
                    { BuildPropertyNameOutputPath, this.BuildFolder }
                };

            foreach (var buildProperty in buildProperties)
            {
                buildParameters[buildProperty.Key] = buildProperty.Value;
            }

            var request = new BuildRequestData(solutionPath, buildParameters, null, new[] { DefaultBuildTarget }, null);
            var parms = new BuildParameters();
            BuildResult result = BuildManager.DefaultBuildManager.Build(parms, request);
            return result.OverallResult == BuildResultCode.Success;
        }

        private bool PrepareBuildFolder()
        {
            // create folder if it does not exist
            if (this.filesystemAccessor.DirectoryExists(this.BuildFolder) == false)
            {
                return this.filesystemAccessor.CreateDirectory(this.BuildFolder);
            }

            // cleanup existing folder
            if (this.filesystemAccessor.DeleteFolder(this.BuildFolder))
            {
                return this.filesystemAccessor.CreateDirectory(this.BuildFolder);
            }

            // build folder could not be cleaned
            return false;
        }

        private string GetBuildFolderPath()
        {
            return Path.GetFullPath(this.applicationInformation.BuildFolder);
        }
    }
}
namespace NuDeploy.Core.Services.AssemblyResourceAccess
{
    public class AssemblyFileResourceInfo
    {
        public AssemblyFileResourceInfo(string resourceName, string resourcePath)
        {
            this.ResourceName = resourceName;
            this.ResourcePath = resourcePath;
        }

        public string ResourceName { get; set; }

        public string ResourcePath { get; set; }
    }
}
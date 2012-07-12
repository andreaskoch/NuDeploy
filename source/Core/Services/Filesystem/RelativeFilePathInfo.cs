namespace NuDeploy.Core.Services.Filesystem
{
    public class RelativeFilePathInfo
    {
        public RelativeFilePathInfo(string absoluteFilePath, string relativeFilePath)
        {
            this.AbsoluteFilePath = absoluteFilePath;
            this.RelativeFilePath = relativeFilePath;
        }

        public string AbsoluteFilePath { get; set; }

        public string RelativeFilePath { get; set; }
    }
}
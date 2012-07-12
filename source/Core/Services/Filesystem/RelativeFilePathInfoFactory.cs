namespace NuDeploy.Core.Services.Filesystem
{
    public class RelativeFilePathInfoFactory : IRelativeFilePathInfoFactory
    {
        public RelativeFilePathInfo GetRelativeFilePathInfo(string absoluteFilePath, string basePath)
        {
            if (absoluteFilePath.StartsWith(basePath))
            {
                string relativePath = absoluteFilePath.Replace(basePath, string.Empty).TrimStart('\\');
                return new RelativeFilePathInfo(absoluteFilePath, relativePath);
            }

            return null;
        }
    }
}
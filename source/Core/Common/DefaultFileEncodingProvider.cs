using System.Text;

namespace NuDeploy.Core.Common
{
    public class DefaultFileEncodingProvider : IEncodingProvider
    {
        public Encoding GetEncoding()
        {
            return Encoding.UTF8;
        }
    }
}
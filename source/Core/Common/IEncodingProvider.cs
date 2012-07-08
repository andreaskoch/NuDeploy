using System.Text;

namespace NuDeploy.Core.Common
{
    public interface IEncodingProvider
    {
        Encoding GetEncoding();
    }
}
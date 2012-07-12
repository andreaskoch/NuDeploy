using System.Text;

namespace NuDeploy.Core.Common.FileEncoding
{
    public interface IEncodingProvider
    {
        Encoding GetEncoding();
    }
}
using System.IO;
using System.Text;

namespace NuDeploy.Tests
{
    public class TestUtilities
    {
        public static StreamReader GetStreamReaderForText(string text)
        {
            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(text)));
        }
    }
}
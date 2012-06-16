using System.Collections.Generic;

namespace NuDeploy.Core.Common
{
    public interface IUserInterface
    {
        void Show(string messageFormatString, params object[] args);

        void ShowKeyValueStore(IDictionary<string, string> keyValueStore);

        void ShowKeyValueStore(IDictionary<string, string> keyValueStore, int indentation);
    }
}
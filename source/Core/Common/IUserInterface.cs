using System.Collections.Generic;

namespace NuDeploy.Core.Common
{
    public interface IUserInterface
    {
        string GetInput();

        void Write(string text);

        void WriteLine(string text);

        void ShowIndented(string text, int marginLeft);

        void ShowLabelValuePair(string label, string value, int distanceBetweenLabelAndValue);

        void ShowKeyValueStore(IDictionary<string, string> keyValueStore, int distanceBetweenColumns, int indentation);

        void ShowKeyValueStore(IDictionary<string, string> keyValueStore, int distanceBetweenColumns);
    }
}
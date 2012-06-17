using System.Collections.Generic;

namespace NuDeploy.Core.Common
{
    public interface IUserInterface
    {
        void ShowIndented(string text, int marginLeft);

        void Show(string text);

        void ShowLabelValuePair(string label, string value, int distanceBetweenLabelAndValue);

        void ShowKeyValueStore(IDictionary<string, string> keyValueStore, int distanceBetweenColumns, int indentation);

        void ShowKeyValueStore(IDictionary<string, string> keyValueStore, int distanceBetweenColumns);
    }
}
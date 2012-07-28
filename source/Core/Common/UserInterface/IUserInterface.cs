using System.Collections.Generic;

namespace NuDeploy.Core.Common.UserInterface
{
    public interface IUserInterface
    {
        string UserInterfaceContent { get; }

        string GetInput();

        void Write(string text);

        void WriteLine(string text);

        void ShowIndented(string text, int marginLeft);

        void ShowLabelValuePair(string label, string value, int distanceBetweenLabelAndValue);

        void ShowKeyValueStore(IDictionary<string, string> keyValueStore, int distanceBetweenColumns, int indentation);

        void ShowKeyValueStore(IDictionary<string, string> keyValueStore, int distanceBetweenColumns);
    }
}
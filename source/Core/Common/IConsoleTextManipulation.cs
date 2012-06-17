namespace NuDeploy.Core.Common
{
    public interface IConsoleTextManipulation
    {
        string WrapLongTextWithHangingIndentation(string text, int maxWidth, int indentation);

        string IndentText(string text, int windowWidth, int marginLeft);
    }
}
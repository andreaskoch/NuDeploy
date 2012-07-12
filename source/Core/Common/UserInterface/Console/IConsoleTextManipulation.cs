namespace NuDeploy.Core.Common.UserInterface.Console
{
    public interface IConsoleTextManipulation
    {
        string WrapText(string text, int maxWidth);

        string WrapLongTextWithHangingIndentation(string text, int maxWidth, int indentation);

        string IndentText(string text, int windowWidth, int marginLeft);
    }
}
using System;

namespace NuDeploy.Core.Common
{
    public class ConsoleUserInterface : IUserInterface
    {
        public void Show(string messageFormatString, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine(messageFormatString);
            }
            else
            {
                Console.WriteLine(messageFormatString, args);
            }
        }
    }
}
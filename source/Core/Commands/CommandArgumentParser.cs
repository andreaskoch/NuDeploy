using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NuDeploy.Core.Commands
{
    public class CommandArgumentParser : ICommandArgumentParser
    {
        private readonly Regex namedArgumentRegex = new Regex("^(--|-|/)(\\w+)=(['\"]?)([^\\3]+?)\\3$");

        private readonly Regex switchArgumentRegex = new Regex("^-(\\w+)$");

        public IEnumerable<KeyValuePair<string, string>> ParseParameters(IEnumerable<string> commandArguments)
        {
            if (commandArguments == null)
            {
                throw new ArgumentNullException("commandArguments");
            }

            var parameters = new List<KeyValuePair<string, string>>();

            // find named parameters
            foreach (var commandArgument in commandArguments)
            {
                string argument = commandArgument.Trim();

                // argument is switch
                if (this.switchArgumentRegex.IsMatch(argument))
                {
                    MatchCollection switchMaches = this.switchArgumentRegex.Matches(argument);
                    string switchName = switchMaches[0].Groups[1].Value;

                    parameters.Add(new KeyValuePair<string, string>(switchName, bool.TrueString));
                    continue;
                }

                // named argument
                if (this.namedArgumentRegex.IsMatch(argument))
                {
                    MatchCollection namedArgumentMatches = this.namedArgumentRegex.Matches(argument);
                    string argumentName = namedArgumentMatches[0].Groups[2].Value;
                    string argumentValue = namedArgumentMatches[0].Groups[4].Value;

                    parameters.Add(new KeyValuePair<string, string>(argumentName, argumentValue));
                    continue;
                }
                
                // argument is unnamed
                parameters.Add(new KeyValuePair<string, string>(string.Empty, argument));
            }

            return parameters;
        }
    }
}
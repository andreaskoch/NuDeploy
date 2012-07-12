using System;
using System.Text.RegularExpressions;

namespace NuDeploy.Core.Services.Console
{
    public class CommandArgumentNameMatcher : ICommandArgumentNameMatcher
    {
        private const StringComparison ComparisionMethod = StringComparison.OrdinalIgnoreCase;

        private readonly Regex argumentModifiersRegex = new Regex("^(--|-|/)");

        public bool IsMatch(string fullArgumentName, string suppliedArgumentName)
        {
            if (fullArgumentName == null)
            {
                throw new ArgumentNullException("fullArgumentName");
            }

            if (string.IsNullOrWhiteSpace(suppliedArgumentName))
            {
                return false;
            }

            // prepare argument for comparision
            suppliedArgumentName = suppliedArgumentName.Trim();
            suppliedArgumentName = this.argumentModifiersRegex.Replace(suppliedArgumentName, string.Empty);

            // full match
            if (fullArgumentName.Equals(suppliedArgumentName, ComparisionMethod))
            {
                return true;
            }

            // partial match
            if (fullArgumentName.StartsWith(suppliedArgumentName, ComparisionMethod))
            {
                return true;
            }

            // no match
            return false;
        }
    }
}
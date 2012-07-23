using System;
using System.Collections.Generic;
using System.Linq;

using NuDeploy.Core.Common;

namespace NuDeploy.CommandLine.Commands
{
    public class BuildPropertyParser : IBuildPropertyParser
    {
        public IEnumerable<KeyValuePair<string, string>> ParseBuildPropertiesArgument(string buildProperties)
        {
            if (buildProperties == null)
            {
                throw new ArgumentNullException("buildProperties");
            }

            var results = new List<KeyValuePair<string, string>>();

            var keyValuePairStrings = buildProperties.Split(NuDeployConstants.MultiValueSeperator).Where(p => string.IsNullOrWhiteSpace(p) == false).Select(p => p.Trim());
            foreach (var keyValuePairString in keyValuePairStrings)
            {
                var segments = keyValuePairString.Split('=');
                if (segments.Count() == 2)
                {
                    string key = segments.First().Trim();

                    string value = segments.Last();
                    if (value != null)
                    {
                        value = value.Trim();
                    }

                    results.Add(new KeyValuePair<string, string>(key, value));
                }
            }

            return results;
        }
    }
}
using System.Collections.Generic;

namespace NuDeploy.CommandLine.Commands
{
    public interface IBuildPropertyParser
    {
        IEnumerable<KeyValuePair<string, string>> ParseBuildPropertiesArgument(string buildProperties);
    }
}
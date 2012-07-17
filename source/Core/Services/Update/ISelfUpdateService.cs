using System;

namespace NuDeploy.Core.Services.Update
{
    public interface ISelfUpdateService
    {
        bool SelfUpdate(string exePath, Version version);
    }
}
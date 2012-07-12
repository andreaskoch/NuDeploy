using System;

namespace NuDeploy.Core.Services.Update
{
    public interface ISelfUpdateService
    {
        void SelfUpdate(string exePath, Version version);
    }
}
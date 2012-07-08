using System;

namespace NuDeploy.Core.Services
{
    public interface ISelfUpdateService
    {
        void SelfUpdate(string exePath, Version version);
    }
}
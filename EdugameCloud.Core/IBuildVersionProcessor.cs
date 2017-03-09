using System;

namespace EdugameCloud.Core
{
    public interface IBuildVersionProcessor
    {
        Version ProcessVersion(string folder, string buildSelector);
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Esynctraining.Core.Comparers;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Core
{
    public sealed class BuildVersionProcessor : IBuildVersionProcessor
    {
        private readonly ILogger _logger;


        public BuildVersionProcessor(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public Version ProcessVersion(string folder, string buildSelector)
        {
            if (string.IsNullOrWhiteSpace(folder))
                throw new ArgumentException("Non-empty value expected", nameof(folder));
            if (string.IsNullOrWhiteSpace(buildSelector))
                throw new ArgumentException("Non-empty value expected", nameof(buildSelector));

            if (!Directory.Exists(folder))
            {
                _logger.Warn($"[ProcessVersion] Directory {folder} not found.");
                throw new InvalidOperationException($"[ProcessVersion] Directory {folder} not found.");
            }

            var versions = new List<KeyValuePair<Version, string>>();
            foreach (var file in Directory.GetFiles(folder, buildSelector))
            {
                var fileName = Path.GetFileName(file);
                versions.Add(new KeyValuePair<Version, string>(fileName.GetBuildVersion(), fileName));
            }

            versions.Sort(new BuildVersionComparer());
            var latestVersionFileName = versions.FirstOrDefault();
            _logger.Info(
                $"[ProcessVersion] Selector={buildSelector}, FileName={latestVersionFileName.Value}.");
            return latestVersionFileName.Key;
        }

    }

}


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Esynctraining.Core.Comparers;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Logging;

namespace EdugameCloud.Core
{
    public class BuildVersionProcessor : IBuildVersionProcessor
    {
        private ILogger _logger;

        public BuildVersionProcessor(ILogger logger)
        {
            _logger = logger;
        }

        public Version ProcessVersion(string folder, string buildSelector)
        {
            if (Directory.Exists(folder))
            {
                var versions = new List<KeyValuePair<Version, string>>();

                foreach (var file in Directory.GetFiles(folder, buildSelector))
                {
                    var fileName = Path.GetFileName(file);
                    var version = fileName.GetBuildVersion();
                    versions.Add(new KeyValuePair<Version, string>(version, fileName));
                }

                versions.Sort(new BuildVersionComparer());
                var latestVersionFileName = versions.FirstOrDefault();
                _logger.Info(
                    $"[ProcessVersion] Selector={buildSelector}, FileName={latestVersionFileName.Value}.");
                return latestVersionFileName.Key;
            }

            _logger.Warn($"[ProcessVersion] Directory {folder} not found.");
            //throw?
            return null;
        }
    }
}


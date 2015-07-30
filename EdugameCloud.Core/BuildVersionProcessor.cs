using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Esynctraining.Core.Comparers;
using Esynctraining.Core.Extensions;

namespace EdugameCloud.Core
{
    public static class BuildVersionProcessor
    {
        public static string ProcessVersion(string swfFolder, string buildSelector)
        {
            var folder = HttpContext.Current.Server.MapPath(swfFolder);
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
                return versions.FirstOrDefault().With(x => x.Value);
            }

            return null;
        }

    }

}


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Esynctraining.Pdf.Common
{
    public class ResourceHelper
    {
        /// <summary>
        /// The flush resource to file.
        /// </summary>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public static void FlushResourceToFile(Assembly assembly, string resourceName, string fileName)
        {
            var ums = (UnmanagedMemoryStream)assembly.GetManifestResourceStream(resourceName);
            if (ums != null)
            {
                using (ums)
                {
                    using (var fs = new FileStream(fileName, FileMode.Create))
                    {
                        var buf = new byte[ums.Length];
                        ums.Read(buf, 0, (int)ums.Length);
                        fs.Write(buf, 0, (int)ums.Length);
                    }    
                }
            }
        }
    }
}

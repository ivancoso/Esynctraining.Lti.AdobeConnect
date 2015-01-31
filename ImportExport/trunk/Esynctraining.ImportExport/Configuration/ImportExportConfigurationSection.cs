using System.Configuration;

namespace Esynctraining.ImportExport.Configuration
{
    internal sealed class ImportExportConfigurationSection : ConfigurationSection, IImportExportConfiguration
    {
        [ConfigurationProperty("filesRootFolder", IsRequired = true, DefaultValue = "C:\tmp")]
        public string FilesRootFolder
        {
            get
            {
                return (string)this["filesRootFolder"];
            }
        }

    }

}

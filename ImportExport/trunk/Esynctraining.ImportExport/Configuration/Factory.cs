using System.Configuration;

namespace Esynctraining.ImportExport.Configuration
{
    public static class Factory
    {
        public static IImportExportConfiguration GetSettings()
        {
            return (IImportExportConfiguration)ConfigurationManager.GetSection("Esynctraining_ImportExport");
        }

    }

}

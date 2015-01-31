using System.Collections.Generic;

namespace VaStart.Common.ImportExport.Excel.ImportModels
{
    public interface IFileImportModel
    {
        string SavedFileName { get; set; }

        // Column: name+index
        IDictionary<string, int> FileColumns { get; set; }

        IEnumerable<MappedColumn> AutoMappedColumns { get; set; }

        IEnumerable<MappedColumn> NotAutoMappedColumns { get; set; }

    }
}

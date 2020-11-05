using System.Collections.Generic;

namespace Esynctraining.ImportExport.Excel.ImportModels
{
    public interface IFileImportModel
    {
        // Column: name+index
        IDictionary<string, int> FileColumns { get; set; }

        IEnumerable<MappedColumn> AutoMappedColumns { get; set; }

        IEnumerable<MappedColumn> NotAutoMappedColumns { get; set; }

    }
}

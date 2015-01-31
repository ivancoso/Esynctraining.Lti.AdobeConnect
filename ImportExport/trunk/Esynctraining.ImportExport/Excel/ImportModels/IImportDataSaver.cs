using System.Collections.Generic;

namespace VaStart.Common.ImportExport.Excel.ImportModels
{
    public interface IImportDataSaver<TRoot, TItem>
    {
        TRoot GetRoot();

        bool InsertItems(TRoot root, IEnumerable<TItem> items);

    }

}

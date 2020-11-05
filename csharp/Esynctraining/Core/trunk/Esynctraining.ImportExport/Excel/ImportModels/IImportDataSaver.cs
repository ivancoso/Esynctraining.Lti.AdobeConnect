using System.Collections.Generic;

namespace Esynctraining.ImportExport.Excel.ImportModels
{
    public interface IImportDataSaver<TRoot, TItem>
    {
        TRoot GetRoot();
        
        int InsertItems(TRoot root, IEnumerable<TItem> items);

    }

}

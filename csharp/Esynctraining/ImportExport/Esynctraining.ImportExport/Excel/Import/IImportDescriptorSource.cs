using System.Collections.Generic;

namespace Esynctraining.ImportExport.Excel.Import
{
    public interface IImportDescriptorSource<T>
    {
        IEnumerable<ImportPropertyDescriptor<T>> GetColumns();

    }

}

namespace Esynctraining.ImportExport.Excel.Import
{
    public interface IImportPropertyDescriptor
    {
        string Name { get; }

        string DisplayName { get; }

        bool Required { get; }

        int ExcelFileColumnIndex { get; set; }

    }

}

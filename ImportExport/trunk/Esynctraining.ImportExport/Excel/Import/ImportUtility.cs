using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExcelHandler;
using ExcelHandler.Interfaces;
using Esynctraining.ImportExport.Configuration;

namespace Esynctraining.ImportExport.Excel.Import
{
    public static class ImportUtility
    {
        /// <summary>
        /// Deletes file. Doesn't throw an exception.
        /// </summary>
        /// <param name="path">File to delete.</param>
        public static void DeleteFile(string path)
        {
            //Check.Argument.IsNotEmpty(path, "path");

            try
            {
                File.Delete(path);
            }
            catch
            {
                // NOTE: it doesn't matter that we can't delete file.
            }
        }

        public static void CleanupExportImportDirectory()
        {
        //    DateTime oldFileThreshold = DateTime.Today.AddDays(-1);

        //    // TODO: is it OK to use this settiong in this project???
        //    CleanupDirectory(MyConfigurationSectionGroup.xxSettings.ExportImportFilesFolder,
        //        fileName => new FileInfo(fileName).CreationTime < oldFileThreshold);
        }

        public static string BuildTempFilePath(string fileName)
        {
            return Path.Combine(Factory.GetSettings().FilesRootFolder, Guid.NewGuid() + Path.GetExtension(fileName));
        }

        public static IDictionary<string, int> GetFileHeaders(string fileName)
        {
            //Check.Argument.IsNotEmpty(fileName, "fileName");

            return GetImportedFileHeaders(fileName);            
        }

        private static IDictionary<string, int> GetFileHeaders(IEnumerable<string> headers)
        {
            //Check.Argument.IsNotNull(headers, "headers");

            var result = new Dictionary<string, int>();
            int headerIndex = 0;

            foreach (var header in headers)
            {
                headerIndex++;
                result.Add(header, headerIndex);
            }

            return result;
        }

        public static void DeleteUploadedFileAndCleanDirectory(string filePath)
        {
            //Check.Argument.IsNotEmpty(filePath, "filePath");

            DeleteFile(filePath);
            CleanupExportImportDirectory();
        }


        private static List<T> GetWorksheetData<T>(string fileName,
            IEnumerable<ImportPropertyDescriptor<T>> mappedColumns) where T : new()
        {
            //Check.Argument.IsNotEmpty(fileName, "fileName");

            var items = new List<T>();

            using (var excelHandler = ExcelHandlerFactory.Instance.Create(fileName))
            {
                IExcelSheet sheet = excelHandler.GetSheet(1);
                var headers = ExcelUtility.GetWorksheetColumns(sheet);

                int colCount = headers.Count();
                int rowCount = 1000 * 1000;    // TODO: !!! ws.Cells.UsedRange.RowCount;
                const int DataRowStartIndex = 2;    // TODO: constant!!
                for (int rowIndex = DataRowStartIndex; rowIndex <= rowCount; rowIndex++)
                {
                    var item = new T();
                    bool hasAnyNonEmptyValue = false;

                    foreach (var mappedColumn in mappedColumns)
                    {
                        string value = sheet.GetCellValue(rowIndex, ExcelUtility.GetColNameFromIndex(mappedColumn.ExcelFileColumnIndex)).ToString().Trim();
                        if (value.Length > 0)
                        {
                            hasAnyNonEmptyValue = true;
                            mappedColumn.SetValue(item, value);
                        }
                    }

                    if (!hasAnyNonEmptyValue)
                        break;

                    items.Add(item);
                }
            }

            return items;
        }

        public static IEnumerable<TImportedItem> GetWorksheetData<TWorksheetItem, TImportedItem>(
           string fileName,
           IEnumerable<ImportPropertyDescriptor<TWorksheetItem>> mappedColumns,
           Func<IEnumerable<TWorksheetItem>, IEnumerable<TImportedItem>> filterAndTransform,
           out int importedCount,
           out int notImportedCount) where TWorksheetItem : new()
        {
            var batchItems = GetWorksheetData(fileName, mappedColumns);
            var result = filterAndTransform(batchItems);

            importedCount = result.Count();
            notImportedCount = batchItems.Count() - importedCount;

            return result;
        }

        public static IDictionary<string, int> GetImportedFileHeaders(string fileName)
        {
            //Check.Argument.IsNotNull(fileName, "fileName");

            return GetFileHeaders(ExcelUtility.GetWorksheetColumns(fileName));
        }

        private static void CleanupDirectory(string path, Func<string, bool> deleteCondition)
        {
            foreach (var fileName in Directory.GetFiles(path).Where(deleteCondition))
            {
                DeleteFile(fileName);
            }
        }

    }

}

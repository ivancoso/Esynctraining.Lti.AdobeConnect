using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Esynctraining.ImportExport.Excel.Import;

namespace Esynctraining.ImportExport.Excel.ImportModels
{
    public sealed class ImportHelper
    {
        public const int NotMarkedExcelFileColumnIndex = -1;

        public ImportResult ImportProcess<TWorksheetItem, TItem, TRoot>(
            IFileImportModel model,
            IEnumerable<ImportPropertyDescriptor<TWorksheetItem>> descriptors,
            Func<IEnumerable<TWorksheetItem>, IEnumerable<TItem>> itemsTransform,
            IImportDataSaver<TRoot, TItem> dataSaver) where TWorksheetItem : new()
        {
            if (!File.Exists(model.SavedFileName))
            {
                return new ImportResult
                {
                    OperationResult = ImportOperationResult.ErrorOnUploadStage,
                    Message = "File was deleted. Please, reupload it.",
                };
            }

            //SetUserMappedColumns(descriptors, Request);

            // TRICK: force set mapping AUTO
            //model.FileColumns = ImportUtility.GetFileHeaders(model.SavedFileName);
            //FillModelMappedColumns(model, descriptors);

            //if (!ValidateMappedRequiredColumns(descriptors))
            if (model.NotAutoMappedColumns.Any(c => c.Requred))
            {
                IEnumerable<string> notMappedRequiredColumns = model.NotAutoMappedColumns
                    .Where(c => c.Requred)
                    .Select(c => "'" + c.DisplayName + "'");

                return new ImportResult
                {
                    OperationResult = ImportOperationResult.ErrorOnImportStage,
                    Message = string.Format("Required columns ({0}) not found. Please verify that the Microsoft Excel file to be imported meets the guidelines.",
                        string.Join(", ", notMappedRequiredColumns)),
                };
            }

            int importedCount;
            int notImportedCount;

            // NOTE: parse only mapped columns
            IEnumerable<TItem> batchItems = ImportUtility.GetWorksheetData(
                model.SavedFileName,
                descriptors.Where(d => d.ExcelFileColumnIndex != NotMarkedExcelFileColumnIndex),
                itemsTransform,
                out importedCount, out notImportedCount);

            if (batchItems.Any())
            {
                //using (var tran = new TransactionScope())
                //{
                //notInsertedItems = (IEnumerable<int>)dataSaver.InsertItems(batch, batchItems);

                dataSaver.InsertItems(dataSaver.GetRoot(), batchItems);

                //tran.Complete();
                //}

                return new ImportResult
                {
                    OperationResult = ImportOperationResult.CompletedSuccessfully,
                    Message = string.Format("Successfuly imported: '{0}' records, not imported: '{1}' records.", importedCount, notImportedCount),
                };
            }
            else
            {
                return new ImportResult
                {
                    OperationResult = ImportOperationResult.CompletedSuccessfully,
                    Message = "There is no data to upload. Please verify that the Microsoft Excel file to be imported meets the guidelines.",
                };
                // TODO:UserMessages.AddWarningMessage(TempData,
                //"There is no data to upload. Please verify that the Microsoft Excel file to be imported meets the guidelines.");
            }
        }

        public static void InitImportModel(
            IFileImportModel emptyImport,
            string savedFilePath,
            IEnumerable<IImportPropertyDescriptor> descriptors)
        {
            //Check.Argument.IsNotEmpty(savedFilePath, "savedFilePath");
            //Check.Argument.IsNotNull(descriptors, "descriptors");

            var importModel = emptyImport;
            importModel.SavedFileName = savedFilePath;
            //importModel.OriginFileName = Path.GetFileName(postedFile.FileName);
            importModel.FileColumns = ImportUtility.GetImportedFileHeaders(savedFilePath);

            AutoMapColumns(descriptors, importModel.FileColumns);
            FillModelMappedColumns(importModel, descriptors);

            // TODO:
            //importModel.ValidationRules = GetValidationRules(descriptors);
        }

        private static void AutoMapColumns(IEnumerable<IImportPropertyDescriptor> descriptors, IDictionary<string, int> importedFileColumnsHeaders)
        {
            //Check.Argument.IsNotNull(importedFileColumnsHeaders, "importedFileColumnsHeaders");
            //Check.Argument.IsNotNull(descriptors, "descriptors");

            foreach (var pi in descriptors)
            {
                int value;
                if (importedFileColumnsHeaders.TryGetValue(pi.DisplayName, out value))
                {
                    pi.ExcelFileColumnIndex = value;
                }
            }
        }

        private static void FillModelMappedColumns(IFileImportModel model,
            IEnumerable<IImportPropertyDescriptor> descriptors)
        {
            //Check.Argument.IsNotNull(model, "model");
            //Check.Argument.IsNotNull(descriptors, "descriptors");

            IDictionary<string, int> importedFileColumnsHeaders = model.FileColumns;

            var notAutoMappedColumns = new List<MappedColumn>();
            var autoMappedColumns = new List<MappedColumn>();

            foreach (var pi in descriptors)
            {
                var displayName = pi.DisplayName;

                var mappedColumn = new MappedColumn
                {
                    Name = pi.Name,
                    DisplayName = displayName,
                    SelectedValue = pi.ExcelFileColumnIndex,
                    Requred = pi.Required,
                };

                // TODO: why not check pi.ExcelFileColumnIndex ??
                if (importedFileColumnsHeaders.ContainsKey(displayName))
                {
                    autoMappedColumns.Add(mappedColumn);
                }
                else
                {
                    notAutoMappedColumns.Add(mappedColumn);
                }
            }

            model.AutoMappedColumns = autoMappedColumns.AsReadOnly();
            model.NotAutoMappedColumns = notAutoMappedColumns.AsReadOnly();
        }

        private static bool ValidateMappedRequiredColumns(IEnumerable<IImportPropertyDescriptor> descriptors)
        {
            bool result = true;

            foreach (var pi in descriptors)
            {
                if (pi.Required
                    && (pi.ExcelFileColumnIndex == NotMarkedExcelFileColumnIndex))
                {
                    result = false;
                }
            }

            return result;
        }

    }
}

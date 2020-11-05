using System;
using System.Collections.Generic;
using System.IO;
using Esynctraining.ImportExport.Excel.Import;
using OfficeOpenXml;

namespace Esynctraining.ImportExport.Excel
{
    internal static class ExcelUtility
    {
        public static IEnumerable<string> GetWorksheetColumns(string fileName, int worksheetIndex)
        {
            //Check.Argument.IsNotEmpty(fileName, "fileName");

            var file = new FileInfo(fileName);
            if (!file.Exists)
                throw new InvalidOperationException($"Excel file '{fileName}' not found");

            using (var package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[worksheetIndex];
                return GetWorksheetColumns(worksheet);
            }
        }

        public static IEnumerable<string> GetWorksheetColumns(ExcelWorksheet sheet)
        {
            var headers = new List<string>();

            // TRICK: no more only
            for (int columnIndex = 1; columnIndex <= 100; columnIndex++)
            {
                ExcelRange headerCell = sheet.Cells[ImportUtility.DataRowStartIndex - 1, columnIndex];
                string cellValue = headerCell?.Value?.ToString();
                if (string.IsNullOrWhiteSpace(cellValue))
                {
                    break;
                }

                headers.Add(cellValue);
            }

            return headers;
        }

        //public static string GetColNameFromIndex(int columnNumber)
        //{
        //    int dividend = columnNumber;
        //    string columnName = string.Empty;
        //    int modulo;

        //    while (dividend > 0)
        //    {
        //        modulo = (dividend - 1) % 26;
        //        columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
        //        dividend = (int)((dividend - modulo) / 26);
        //    }

        //    return columnName;
        //}

    }

}

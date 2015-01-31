using System;
using System.Collections.Generic;
using ExcelHandler;
using ExcelHandler.Interfaces;

namespace Esynctraining.ImportExport.Excel
{
    internal static class ExcelUtility
    {
        public static IEnumerable<string> GetWorksheetColumns(string fileName)
        {
            //Check.Argument.IsNotEmpty(fileName, "fileName");

            using (var excelHandler = ExcelHandlerFactory.Instance.Create(fileName))
            {
                IExcelSheet sheet = excelHandler.GetSheet(1);
                return GetWorksheetColumns(sheet);
            }
        }

        public static IEnumerable<string> GetWorksheetColumns(IExcelSheet sheet)
        {
            var headers = new List<string>();

            // TRICK: no more only
            for (int columnIndex = 1; columnIndex <= 100; columnIndex++)
            {
                object headerValue = sheet.GetCellValue(1, GetColNameFromIndex(columnIndex));
                if ((headerValue == null) || string.IsNullOrWhiteSpace(headerValue.ToString()))
                {
                    break;
                }

                headers.Add(headerValue.ToString());
            }

            return headers;
        }

        public static string GetColNameFromIndex(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

    }

}

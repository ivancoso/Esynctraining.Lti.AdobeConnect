using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Core.Domain.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace EdugameCloud.MVC.Services
{
    public class MngQuizReportService : IExtendedReportService
    {
        public const int StartUserColumn = 5;
        // Helmsley Fraser report
        public byte[] GetExcelExtendedReportBytes(IEnumerable<ExtendedReportDto> dtos)
        {
            var result = new byte[0];

            var questions = dtos.SelectMany(x => x.Questions).GroupBy(x => x.Id).Select(x => x.First()).OrderBy(x => x.QuestionOrder);
            using (ExcelPackage pck = new ExcelPackage())
            {
                int sessionNumber = 1;
                foreach (var sessionResult in dtos)
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add($"{sessionResult.Name}");
                    ws.Cells[1, 1].Value = "Name";
                    ws.Cells[1, 1].Style.Font.Bold = true;
                    ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));
                    ws.Column(1).Width = 40;
                    int startUserRow = 2;

                    foreach (var quizPlayerDTO in sessionResult.ReportResults)
                    {
                        ws.Cells[startUserRow++, 1].Value = quizPlayerDTO.ParticipantName;
                    }

                    int questionOrder = 1;
                    int startQuestionColumn = 1;

                    foreach (var q in questions)
                    {
                        var internalDict = new Dictionary<int, int>();
                        List<string> correctAnswers = new List<string>();
                        foreach (var distractor in q.Distractors)
                        {
                            internalDict.Add(distractor.Id, 0);
                            if (distractor.IsCorrect.GetValueOrDefault())
                            {
                                correctAnswers.Add(Regex.Replace(distractor.DistractorName, "<[^>]*(>|$)", string.Empty).Replace("&nbsp;", " "));
                            }
                        }
                        startUserRow = 2;
                        string questionTitle =
                            $"Question {questionOrder++} - {Regex.Replace(q.QuestionName, "<[^>]*(>|$)", string.Empty).Replace("&nbsp;", " ")}" 
                            + $"(correct answer(s): {string.Join(",", correctAnswers)})";
                        //correct answer - B
                        startQuestionColumn = startQuestionColumn + 1;
                        ws.Column(startQuestionColumn).Width = 30;
                        ws.Cells[1, startQuestionColumn].Value = questionTitle;
                        ws.Cells[1, startQuestionColumn].Style.Font.Bold = true;
                        ws.Cells[1, startQuestionColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        ws.Cells[1, startQuestionColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217,217,217));

                        foreach (var qr in sessionResult.ReportResults)
                        {
                            var qqr =
                                qr.Results.FirstOrDefault(x => x.QuestionId == q.Id);

                            switch (sessionResult.SubModuleItemType)
                            {

                                case SubModuleItemType.Quiz:
                                case SubModuleItemType.Test:
                                    if (qqr == null)
                                    {
                                        ws.Cells[startUserRow, startQuestionColumn].Value = "N/A"; // No Answer
                                        ws.Cells[startUserRow, startQuestionColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        ws.Cells[startUserRow, startQuestionColumn].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                    }
                                    else
                                    {
                                        ws.Cells[startUserRow, startQuestionColumn].Value = qqr.IsCorrect? "Correct" : "Incorrect";
                                        if (!qqr.IsCorrect)
                                        {
                                            ws.Cells[startUserRow, startQuestionColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            ws.Cells[startUserRow, startQuestionColumn].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                        }
                                    }
                                    ws.Cells[startUserRow, startQuestionColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    startUserRow++;
                                    break;
                            }
                        }
                    }
                    startQuestionColumn++;
                    ws.Cells[1, startQuestionColumn].Value = "Attestation";
                    ws.Cells[1, startQuestionColumn].Style.Font.Bold = true;
                    ws.Cells[1, startQuestionColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells[1, startQuestionColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, startQuestionColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(217, 217, 217));
                }

                result = pck.GetAsByteArray();
            }

            return result;
        }
    }
}

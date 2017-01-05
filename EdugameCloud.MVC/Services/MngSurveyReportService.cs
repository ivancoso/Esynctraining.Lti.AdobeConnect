using System;
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
    public class MngSurveyReportService : IExtendedReportService
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
                    ws.Cells[1, 1].Value = "Attendee";
//                    ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
//                    ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 0, 0));
                    ws.Cells[1, 1].AutoFilter = true;
                    ws.Column(1).Width = 40;
                    int startUserRow = 2;

                    foreach (var quizPlayerDTO in sessionResult.ReportResults)
                    {
                        ws.Cells[startUserRow, 1].Value = quizPlayerDTO.ParticipantName;
                        startUserRow++;
                    }

                    int questionOrder = 1;
                    int startQuestionColumn = 0;

                    foreach (var q in questions)
                    {
                        var internalDict = new Dictionary<int, int>();
                        foreach (var distractor in q.Distractors)
                        {
                            internalDict.Add(distractor.Id, 0);
                        }
                        startUserRow = 2;
                        string questionTitle =
                            $"Question {questionOrder++} - {Regex.Replace(q.QuestionName, "<[^>]*(>|$)", string.Empty).Replace("&nbsp;", " ")}";
                        startQuestionColumn = startQuestionColumn + 2;
                        ws.Column(startQuestionColumn).Width = 30;
                        ws.Cells[1, startQuestionColumn].Value = questionTitle;
//                        ws.Cells[1, startQuestionColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
//                        ws.Cells[1, startQuestionColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 0, 0));
//                        ws.Cells[1, startQuestionColumn+1].Style.Fill.PatternType = ExcelFillStyle.Solid;
//                        ws.Cells[1, startQuestionColumn].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 0, 0));
                        ws.Cells[1, startQuestionColumn].AutoFilter = true;

                        foreach (var qr in sessionResult.ReportResults)
                        {
                            var qqr =
                                qr.Results.FirstOrDefault(x => x.QuestionId == q.Id);

                            switch (sessionResult.SubModuleItemType)
                            {

                                case SubModuleItemType.Survey:
                                    if (qqr == null || !qqr.DistractorIds.Any())
                                    {
                                        ws.Cells[startUserRow++, startQuestionColumn].Value = "no answer"; // No Answer

                                    }
                                    else
                                    {
                                        var firstDistractor = qqr.DistractorIds.First();
                                        internalDict[firstDistractor] += 1;
                                        var answer = qqr.Answer;
                                        ws.Cells[startUserRow++, startQuestionColumn].Value = answer; 
                                    }
                                    break;
                            }


                        }
                        startUserRow++;
                        ws.Cells[startUserRow, startQuestionColumn].Value = "Answer";
                        ws.Cells[startUserRow, startQuestionColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[startUserRow, startQuestionColumn + 1].Value = "Number";
                        ws.Cells[startUserRow++, startQuestionColumn + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        foreach (var d in q.Distractors)
                        {
                            ws.Cells[startUserRow, startQuestionColumn].Value = Regex.Replace(d.DistractorName, "<[^>]*(>|$)", string.Empty).Replace("&nbsp;", " ");
                            ws.Cells[startUserRow++, startQuestionColumn+1].Value = internalDict[d.Id];
                        }
                        ws.Cells[startUserRow, startQuestionColumn].Value = "Grand Total";
                        ws.Cells[startUserRow, startQuestionColumn].Style.Font.Bold = true;
                        ws.Cells[startUserRow, startQuestionColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[startUserRow, startQuestionColumn + 1].Value = internalDict.Values.Sum();
                        ws.Cells[startUserRow, startQuestionColumn + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[startUserRow++, startQuestionColumn+1].Style.Font.Bold = true;

                    }

                    ws.Cells[1, 1, 1, 1 + questions.Count()*2].AutoFilter = true;
                    ws.Cells[1, 1, 1, 1 + questions.Count()*2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[1, 1, 1, 1 + questions.Count()*2].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(192, 0, 0));
                    ws.Cells[1, 1, 1, 1 + questions.Count()*2].Style.Font.Color.SetColor(Color.White);
                    ws.Cells[1, 1, 1, 1 + questions.Count()*2].Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                    ws.Cells[1, 1, 1, 1 + questions.Count()*2].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
//                    ws.Cells[1, 1, 1, 1 + questions.Count()*2].Style.WrapText = true;
                    ws.Row(1).CustomHeight = true;
                    ws.Row(1).Height = 15;
                    ws.Cells[1, 1, 1, 1 + questions.Count()*2].Style.WrapText = true;
                    //                    ws.Column(4).Width = 70.0;
                    //                    ws.Column(2).Width = 50.0;
                }

                result = pck.GetAsByteArray();
            }

            return result;
        }
    }
}

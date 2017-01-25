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
                        List<string> correctAnswers = new List<string>();
                        int answerOrder = Convert.ToInt32('a');

                        foreach (var distractor in q.Distractors.OrderBy(x => x.DistractorOrder))
                        {
                            if (distractor.IsCorrect.GetValueOrDefault())
                            {
                                correctAnswers.Add(Convert.ToString((char)answerOrder));
                            }

                            answerOrder++;
                        }
                        startUserRow = 2;
                        string questionTitle =
                            $"Question {questionOrder++} - (correct answer{(correctAnswers.Count > 1 ? "s" :"")} {string.Join(",", correctAnswers)})";
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
                                        string answerResultCellValue;
                                        if (qqr.IsCorrect)
                                        {
                                            answerResultCellValue = "Correct";
                                        }
                                        else
                                        {
                                            ws.Cells[startUserRow, startQuestionColumn].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                            ws.Cells[startUserRow, startQuestionColumn].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                                            answerOrder = Convert.ToInt32('a');
                                            var incorrectAnswers = new List<string>();
                                            if(qqr.DistractorIds != null)
                                            foreach (var distractor in q.Distractors.OrderBy(x => x.DistractorOrder))
                                            {
                                                if (qqr.DistractorIds.Any(x => x == distractor.Id))
                                                {
                                                    incorrectAnswers.Add(Convert.ToString((char)answerOrder));
                                                }

                                                answerOrder++;
                                            }

                                            answerResultCellValue = "Incorrect; " + string.Join(",", incorrectAnswers);
                                        }

                                        ws.Cells[startUserRow, startQuestionColumn].Value = answerResultCellValue;
                                    }

                                    ws.Cells[startUserRow, startQuestionColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    startUserRow++;
                                    break;
                            }
                        }
                    }
                }

                result = pck.GetAsByteArray();
            }

            return result;
        }
    }
}

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
    public class ExtendedReportService : IExtendedReportService
    {
        public const int StartUserColumn = 5;
        // Helmsley Fraser report
        public byte[] GetExcelExtendedReportBytes(IEnumerable<ExtendedReportDto> dtos)
        {
            var result = new byte[0];
            using (ExcelPackage pck = new ExcelPackage())
            {
                int sessionNumber = 1;
                foreach (var sessionResult in dtos)
                {
                    //Create the worksheet
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add($"Session #{sessionNumber++}");
                    ws.Cells[1, 1].Value = "Test title";
                    ws.Cells[1, 2, 1, 4].Merge = true;

                    var questionsCount = sessionResult.Questions.Count();
                    ws.Cells[1, 2].Value = $"{sessionResult.QuizName} - {questionsCount} question{(questionsCount == 1 ? "" : "s")}";
                    ws.Cells[2, 4].Value = $"{questionsCount} question{(questionsCount == 1 ? "" : "s")}";
                    ws.Cells[2, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //Set Pattern for the background to Solid
                    ws.Cells[2, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(146, 208, 80));
                    int startUserPartColumn = StartUserColumn;

                    foreach (var quizPlayerDTO in sessionResult.QuizResults)
                    {
                        ws.Cells[2, startUserPartColumn++].Value = quizPlayerDTO.ParticipantName;
                    }
                    ws.Cells[3, 1].Value = "Question #";
                    ws.Cells[3, 2].Value = "Question";
                    ws.Cells[3, 3].Value = "Answer choice";
                    ws.Cells[3, 4].Value = "Answers";

                    int startRow = 4;
                    int endRow = 4;
                    int startUserRowCorrect = 0;
                    int questionOrder = 1;
                    foreach (var q in sessionResult.Questions)
                    {
                        List<int> correctRows = new List<int>();
                        startUserPartColumn = StartUserColumn;
                        int answerOrder = Convert.ToInt32('a');
                        switch ((QuestionTypeEnum)q.QuestionType.Id)
                        {
                            case QuestionTypeEnum.SingleMultipleChoiceText:
                                foreach (var distractor in q.Distractors)
                                {
                                    if (distractor.IsCorrect.GetValueOrDefault())
                                    {
                                        if (!correctRows.Any())
                                        {
                                            startUserRowCorrect = endRow;
                                        }

                                        correctRows.Add(endRow);
                                    }

                                    ws.Cells[endRow, 3].Value = Convert.ToString((char)answerOrder);
                                    ws.Cells[endRow++, 4].Value = distractor.DistractorName;
                                    answerOrder++;
                                }
                                break;
                            case QuestionTypeEnum.TrueFalse:
                                ws.Cells[endRow, 3].Value = Convert.ToString((char)answerOrder);
                                ws.Cells[endRow++, 4].Value = "true";
                                answerOrder++;
                                ws.Cells[endRow, 3].Value = Convert.ToString((char)answerOrder);
                                ws.Cells[endRow++, 4].Value = "false";

                                if (q.Distractors.Count == 1)
                                {
                                    startUserRowCorrect = q.Distractors[0].IsCorrect.GetValueOrDefault()
                                        ? endRow - 2
                                        : endRow - 1;
                                    correctRows.Add(startUserRowCorrect);
                                }
                                break;
                            default:
                                endRow++;
                                break;

                        }
                        ws.Cells[endRow++, 3].Value = "No Answer";
                        ws.Cells[endRow++, 3].Value = "Score";
                        if (startUserRowCorrect < startRow)
                        {
                            startUserRowCorrect = endRow - 1; //Score
                            correctRows.Add(startUserRowCorrect);
                        }

                        foreach (var qr in sessionResult.QuizResults)
                        {
                            var qqr =
                                qr.Results.FirstOrDefault(x => x.QuestionId == q.Id);
                            if (qqr != null)
                            {
                                ws.Cells[startUserRowCorrect, startUserPartColumn].Value = qqr.IsCorrect ? 1 : 0;
                            }
                            else
                            {
                                ws.Cells[endRow - 2, startUserPartColumn].Value = 0; // No Answer
                            }

                            startUserPartColumn++;

                            foreach (var correctRow in correctRows)
                            {
                                ws.Cells[correctRow, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[correctRow, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(146, 208, 80));
                            }
                        }

                        ws.Cells[startRow, 1, endRow - 1, 1].Merge = true;
                        ws.Cells[startRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[startRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[startRow, 1].Value = questionOrder++;
                        ws.Cells[startRow, 2, endRow - 1, 2].Merge = true;
                        ws.Cells[startRow, 2].Style.WrapText = true;
                        ws.Cells[startRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        ws.Cells[startRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        ws.Cells[startRow, 2].Value = Regex.Replace(q.QuestionName, "<[^>]*(>|$)", string.Empty).Replace("&nbsp;", " "); //replace tags
                        startRow = endRow;
                    }

                    ws.Cells[2, StartUserColumn, 2, StartUserColumn + sessionResult.QuizResults.Count()].AutoFitColumns();
                    ws.Column(4).Width = 70.0;
                    ws.Column(2).Width = 50.0;

                    //footer
                    startRow++;
                    ws.Cells[startRow, 4].Value = "# Wrong";
                    ws.Cells[startRow + 1, 4].Value = "Score";

                    for (var i = 0; i < sessionResult.QuizResults.Count(); i++)
                    {
                        ws.Cells[startRow, StartUserColumn + i].FormulaR1C1 = $"={questionsCount}-SUM(R3C:R[-1]C)";
                        ws.Cells[startRow + 1, StartUserColumn + i].FormulaR1C1 = $"=ROUNDUP(({questionsCount}-R[-1]C)/{questionsCount}*100, 0)";
                    }
                }

                result = pck.GetAsByteArray();
            }

            return result;
        }
    }
}

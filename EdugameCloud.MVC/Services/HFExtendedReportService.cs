using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using EdugameCloud.Core.Domain.DTO;
using EdugameCloud.Core.Domain.Entities;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace EdugameCloud.MVC.Services
{
    /// <summary>
    /// report template for Helmsley Fraser
    /// </summary>
    public class HFExtendedReportService : IExtendedReportService
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
                    ws.Cells[1, 1].Value = "Test/Survey title";
                    ws.Cells[1, 2, 1, 4].Merge = true;

                    var questionsCount = sessionResult.Questions.Count();
                    ws.Cells[1, 2].Value = $"{sessionResult.Name} - {questionsCount} question{(questionsCount == 1 ? "" : "s")}";
                    ws.Cells[2, 4].Value = $"{questionsCount} question{(questionsCount == 1 ? "" : "s")}";
                    ws.Cells[2, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //Set Pattern for the background to Solid
                    ws.Cells[2, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(146, 208, 80));
                    int startUserPartColumn = StartUserColumn;

                    foreach (var quizPlayerDTO in sessionResult.ReportResults)
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
                    List<int> excludeRowsFromSum = new List<int>();
                    foreach (var q in sessionResult.Questions)
                    {
                        List<int> correctRows = new List<int>();
                        startUserPartColumn = StartUserColumn;
                        int answerOrder = Convert.ToInt32('a');
                        var answerRows = new Dictionary<int, int>(); //key - distractorid, value - row
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
                                    answerRows.Add(distractor.Id, endRow);
                                    ws.Cells[endRow, 3].Value = Convert.ToString((char)answerOrder);
                                    ws.Cells[endRow++, 4].Value = Regex.Replace(distractor.DistractorName, "<[^>]*(>|$)", string.Empty).Replace("&nbsp;", " ");
                                    answerOrder++;
                                }
                                break;
                            // not used, question types are filtered earlier
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
                            case QuestionTypeEnum.Speedometer:
                                excludeRowsFromSum.Add(endRow);
                                try
                                {
                                    var distractor = q.Distractors.First();
                                    var xmlDoc = new XmlDocument();
                                    xmlDoc.LoadXml(distractor.DistractorName); //<meter trackMin="0" trackMax="8" tickInterval="1" correct="3" unit="hn" range="3"/>
                                    XmlElement root = xmlDoc.DocumentElement;

                                    string description = $"Values: {root.Attributes["trackMin"].Value}-{root.Attributes["trackMax"].Value}, Correct: {root.Attributes["correct"].Value}";
                                    if (root.HasAttribute("range"))
                                    {
                                        if(int.TryParse(root.Attributes["correct"].Value, out int intCorrect) && int.TryParse(root.Attributes["range"].Value, out int intRange))
                                        {
                                            description += $"-{intCorrect + intRange}";
                                        }
                                    }
                                    ws.Cells[endRow, 4].Value = description;
                                }
                                catch(Exception e)
                                {
                                    //log
                                }
                                ws.Cells[endRow++, 3].Value = "Answer choice";
                                break;
                            default:
                                endRow++;
                                break;

                        }
                        ws.Cells[endRow++, 3].Value = "No Answer";
                        if (sessionResult.SubModuleItemType == SubModuleItemType.Quiz)
                        {
                            ws.Cells[endRow++, 3].Value = "Score";
                            if (startUserRowCorrect < startRow)
                            {
                                startUserRowCorrect = endRow - 1; //Score
                                correctRows.Add(startUserRowCorrect);
                            }
                        }

                        if(sessionResult.SubModuleItemType == SubModuleItemType.Quiz)
                        {
                            foreach (var correctRow in correctRows)
                            {
                                ws.Cells[correctRow, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                ws.Cells[correctRow, 4].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(146, 208, 80));
                            }
                        }

                        foreach (var qr in sessionResult.ReportResults)
                        {
                            var qqr =
                                qr.Results.FirstOrDefault(x => x.QuestionId == q.Id);

                            switch (sessionResult.SubModuleItemType)
                            {
                                case SubModuleItemType.Quiz:
                                    if (qqr != null)
                                    {
                                        if ((QuestionTypeEnum)q.QuestionType.Id == QuestionTypeEnum.Speedometer)
                                        {
                                            if (int.TryParse(qqr.Answer, out int intAnswer))
                                                ws.Cells[endRow - 3, startUserPartColumn].Value = intAnswer;
                                            else
                                                ws.Cells[endRow - 3, startUserPartColumn].Value = qqr.Answer; //answer choice
                                        }

                                        ws.Cells[startUserRowCorrect, startUserPartColumn].Value = 
                                            qqr.IsCorrect ? 1 : 0;
                                    }
                                    else
                                    {
                                        ws.Cells[endRow - 2, startUserPartColumn].Value = 0; // No Answer
                                    }
                                    
                                    break;
                                case SubModuleItemType.Survey:
                                    if (qqr == null || !qqr.DistractorIds.Any())
                                    {
                                        ws.Cells[endRow - 1, startUserPartColumn].Value = "X"; // No Answer

                                    }
                                    else
                                    {
                                        foreach (var distractorId in qqr.DistractorIds)
                                        {
                                            if (answerRows.ContainsKey(distractorId))
                                            {
                                                ws.Cells[answerRows[distractorId], startUserPartColumn].Value = "X";
                                            }
                                        }
                                    }
                                    break;
                            }

                            startUserPartColumn++;
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

                    ws.Cells[2, StartUserColumn, 2, StartUserColumn + sessionResult.ReportResults.Count()].AutoFitColumns();
                    ws.Column(4).Width = 70.0;
                    ws.Column(2).Width = 50.0;

                    if (sessionResult.SubModuleItemType == SubModuleItemType.Quiz)
                    {
                        //footer
                        startRow++;
                        ws.Cells[startRow, 4].Value = "# Wrong";
                        ws.Cells[startRow + 1, 4].Value = "Score";

                        for (var i = 0; i < sessionResult.ReportResults.Count(); i++)
                        {
                            var formulaR1C1 = $"={questionsCount}-SUM(R3C:R[-1]C)";
                            foreach(var excludeRaw in excludeRowsFromSum)
                            {
                                formulaR1C1 += $"+R{excludeRaw}C";
                            }
                            ws.Cells[startRow, StartUserColumn + i].FormulaR1C1 = formulaR1C1;
                            ws.Cells[startRow + 1, StartUserColumn + i].FormulaR1C1 =
                                $"=ROUNDUP(({questionsCount}-R[-1]C)/{questionsCount}*100, 0)";
                        }
                    }
                }

                result = pck.GetAsByteArray();
            }

            return result;
        }
    }
}

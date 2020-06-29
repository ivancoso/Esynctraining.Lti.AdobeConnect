using System;
using System.Collections.Generic;
using System.Net;
using EdugameCloud.Lti.DTO;
using Esynctraining.Lti.Lms.Common.Dto;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.Sakai
{
    internal class SakaiFillInMultipleBlanksParser : SakaiCommonQuestionParser
    {
        public SakaiFillInMultipleBlanksParser(BBAssessmentDTO td) :base(td)
        {
           
        }

        public override LmsQuestionDTO ParseQuestion(BBQuestionDTO dto)
        {
            var baseResult = base.ParseQuestion(dto);
            baseResult.question_text = dto.questionText;
            return baseResult;
        }

        //protected class FillInTheBlankAnswer
        //{
        //    public string text { get; set; }
        //    public string subType { get; set; }
        //    public bool caseSensitive { get; set; }
        //}

        protected override List<AnswerDTO> ParseAnswers(BBQuestionDTO q)
        {
            //var ret = new List<AnswerDTO>();
            //int i = 0;
            //var answersList = (JArray)q.answersList;
            //var items = answersList.ToObject<List<FillInTheBlankAnswer>>();
            ////foreach (var answer in items)
            //{
            //    var dto = ParseFillInBlankAnswer(items, "don't know", i);
            //    ret.Add(dto);
            //    //i++;
            //}

            //return ret;
            var ret = new List<AnswerDTO>();
            int i = 0;
            foreach (var answer in (JObject)q.answersList)
            {
                var dto = ParseFillInBlankAnswer(answer.Value.ToObject<List<string>>(), answer.Key, i);
                ret.Add(dto);
                i++;
            }

            if (q.variableSets is JObject)
            {
                var variableSets = q.variableSets as JObject;
                foreach (var set in variableSets)
                {
                    double tolerance = 0;
                    double.TryParse(q.tolerance, out tolerance);

                    var quizAnswer = new AnswerDTO()
                    {
                        id = 0,
                        margin = tolerance,
                        question_text = set.Key,
                        text = DecodeFormula(q.formula),
                        weight = 100
                    };

                    if (set.Value is JObject)
                    {
                        foreach (var variable in set.Value as JObject)
                        {
                            if (variable.Key.Equals("answer"))
                            {
                                quizAnswer.answer = double.Parse(variable.Value.ToString());
                            }
                            else
                            {
                                quizAnswer.variables.Add(new VariableDTO()
                                {
                                    name = variable.Key,
                                    value = variable.Value.ToString()
                                });
                            }
                        }
                    }

                    ret.Add(quizAnswer);

                    break;
                }
                return ret;
            }

            return ret;
        }

        private static string DecodeFormula(string formula)
        {
            if (formula == null)
            {
                return null;
            }
            formula = WebUtility.HtmlDecode(formula);
            formula = formula.Replace("<mi>", "[").Replace("</mi>", "]");
            return formula;
        }

        protected AnswerDTO ParseFillInBlankAnswer(List<string> typedAnswers, string blankName, int blankId)
        {
            var dto = new AnswerDTO()
            {
                id = blankId,
                text = String.Join(SakaiHelper.AnswersSeparator, typedAnswers), 
                blank_id = blankName,
                //question_type = String.Join(SakaiHelper.AnswersSeparator, typedAnswers.Select(a => a.subType)),
                //caseSensitive = typedAnswers.Any(x => x.caseSensitive)
            };

            return dto;
        }
    }
}
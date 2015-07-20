namespace EdugameCloud.Lti.BlackBoard
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Castle.Core.Internal;

    using EdugameCloud.Lti.DTO;
    using EdugameCloud.Lti.Extensions;

    using Newtonsoft.Json.Linq;

    using RestSharp.Contrib;

    internal sealed class BlackboardQuizParser
    {
        private static readonly string[] singleQuestionTypes = new[] {"Multiple Choice", "Opinion Scale"};
        public static int GetBBId(string id)
        {
            return int.Parse(id.TrimStart("_".ToCharArray()).Split('_').First());
        }

        public static LmsQuestionDTO[] ParseQuestions(BBAssessmentDTO td)
        {
            var ret = td.questions == null ? new LmsQuestionDTO[] { }
                                                    : td.questions.Select(
                                                        q =>
                                        new LmsQuestionDTO()
                                        {
                                            question_text = q.text.ClearName(),
                                            question_type = q.type,
                                            is_single = singleQuestionTypes.Any(x => q.type.Equals(x, StringComparison.InvariantCultureIgnoreCase)),
                                            question_name = q.title.ClearName(),
                                            id = GetBBId(q.id),
                                            answers = ParseAnswers(q)
                                        })
                                        .ToArray();

            ret.ForEach(q => q.answers.ForEach(
                a =>
                    {
                        a.text = a.text.ClearName();
                        a.question_text = a.question_text.ClearName();
                    }));

            return ret;
        }

        /// <summary>
        /// The decode formula.
        /// </summary>
        /// <param name="formula">
        /// The formula.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string DecodeFormula(string formula)
        {
            if (formula == null)
            {
                return null;
            }
            formula = HttpUtility.HtmlDecode(formula);
            formula = formula.Replace("<mi>", "[").Replace("</mi>", "]");
            return formula;
        }

        private static List<AnswerDTO> ParseAnswers(BBQuestionDTO q)
        {
            var correctAnswerId = 0;

            if (!string.IsNullOrEmpty(q.answer))
            {
                if (q.answers == null && q.answersList == null)
                {
                    string answerFirstPart = null;
                    if (!string.IsNullOrEmpty(q.answersChoices))
                    {
                        var dotIndex = q.answersChoices.IndexOf(".");
                        answerFirstPart = q.answersChoices.Substring(0, dotIndex);
                    }

                    double margin = 0;
                    if (q.answerRange != null)
                    {
                        double.TryParse(q.answerRange, out margin);
                    }
                    return new List<AnswerDTO>() { new AnswerDTO() { text = q.answer, weight = 100, id = 0, margin = margin, question_text = answerFirstPart } };
                }
                else
                {
                    correctAnswerId = int.Parse(q.answer);
                }
            }

            var ret = new List<AnswerDTO>();

            if (q.answersList is JObject)
            {
                var answersList = q.answersList as JObject;

                if (answersList["image"] != null)
                {
                    var coords = answersList["coord"].ToString();
                    if (coords.Length < 4)
                    {
                        return ret;
                    }
                    var image = answersList["image"].ToString();

                    ret.Add(new AnswerDTO()
                    {
                        text = coords,
                        question_text = image
                    });
                    return ret;
                }

                var i = 0;
                foreach (var answer in answersList)
                {
                    bool isList = false;
                    var stringValue = string.Empty;
                    if (answer.Value is JContainer && answer.Value.Any())
                    {
                        isList = true;
                        stringValue = answer.Value.First().ToString();
                    }
                    else if (answer.Value != null)
                    {
                        stringValue = answer.Value.ToString();
                    }

                    ret.Add(new AnswerDTO()
                    {
                        id = i++,
                        text = isList ? stringValue : answer.Key,
                        blank_id = answer.Key,
                        weight = stringValue.Equals("true", StringComparison.InvariantCultureIgnoreCase) ? 100 : 0
                    });
                }
                return ret;
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

            if (q.answers is JObject)
            {
                var answersList = q.answers as JObject;
                var i = 0;
                if (answersList.Count > 0)
                {
                    foreach (var answer in answersList)
                    {
                        var rightAnswer = answer.Value != null ? answer.Value.ToString() : string.Empty;

                        if (q.choices is JObject)
                        {
                            var currentChoice = (q.choices as JObject)[answer.Key];

                            if (currentChoice != null)
                            {
                                foreach (var option in currentChoice as JObject)
                                {
                                    var answerDto = new AnswerDTO()
                                    {
                                        id = i++,
                                        blank_id = answer.Key,
                                        match_id = option.Key,
                                        text = option.Value.ToString(),
                                        weight = option.Key.Equals(rightAnswer) ? 100 : 0
                                    };

                                    ret.Add(answerDto);
                                }
                            }
                        }
                    }
                }
                else if (q.choices is JObject)
                {
                    foreach (var choice in q.choices as JObject)
                    {
                        foreach (var option in choice.Value as JObject)
                        {
                            var answerDto = new AnswerDTO()
                            {
                                id = i++,
                                blank_id = choice.Key,
                                match_id = option.Key,
                                text = option.Value.ToString()
                            };

                            ret.Add(answerDto);
                        }
                    }
                }
                return ret;
            }

            if (q.answerPhrasesList != null || q.questionWordsList != null)
            {
                var i = 0;
                foreach (var question in q.questionWordsList ?? new string[] { string.Empty })
                {
                    foreach (var phrase in q.answerPhrasesList ?? new string[] { string.Empty })
                    {
                        var answer = string.Format("{0} {1}", question, phrase).Trim();
                        ret.Add(new AnswerDTO() { text = answer, weight = 100, id = i++ });
                    }
                }

                return ret;
            }

            if (q.answersList is JContainer)
            {
                List<string> answers = null;
                if (q.answers is JContainer)
                {
                    answers = (q.answers as JContainer).Select(t => t.ToString()).ToList();
                }
                var answersList = q.answersList as JContainer;
                var i = 0;
                foreach (var answer in answersList)
                {
                    int order = 0;
                    string questionText = null, answerText = null;

                    if (answer is JObject)
                    {
                        foreach (var option in answer as JObject)
                        {
                            questionText = option.Key;
                            answerText = option.Value.ToString();
                            break;
                        }
                    }
                    else
                    {
                        answerText = answer.ToString();
                        int.TryParse(answerText, out order);
                    }

                    ret.Add(new AnswerDTO()
                    {
                        id = i,
                        text = answers != null && answers.Count > i ? answers[i] : answerText,
                        order = order,
                        question_text = questionText,
                        weight = i == correctAnswerId ? 100 : 0
                    });
                    i++;
                }
                return ret;
            }

            return new List<AnswerDTO>() { new AnswerDTO() { text = "no answer", weight = 100, id = 0 } };
        }
    }
}

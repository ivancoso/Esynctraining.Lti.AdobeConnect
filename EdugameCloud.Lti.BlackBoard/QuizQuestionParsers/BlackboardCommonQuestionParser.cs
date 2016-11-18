using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.BlackBoard.QuizQuestionParsers
{
    // todo: split this class to smaller parts for different question types
    public class BlackboardCommonQuestionParser : IBlackboardQuestionParser
    {
        private readonly string[] singleQuestionTypes = new[] { "Multiple Choice", "Opinion Scale" };
        private Func<Dictionary<string, byte[]>> images;

        public BlackboardCommonQuestionParser(BBAssessmentDTO td)
        {
            images = () =>
            {
                var tdImages = td.images as JToken;
                if (tdImages == null) return null;
                var t = tdImages.ToObject<Dictionary<string, string>>();
                return t.ToDictionary(x => x.Key, x => Encoding.UTF8.GetBytes(x.Value));
            };
        }

        public virtual LmsQuestionDTO ParseQuestion(BBQuestionDTO dto)
        {
            var ret = new LmsQuestionDTO()
            {
                //question_text = dto.text.ClearName(),
                question_type = dto.type,
                is_single = singleQuestionTypes.Any(
                    x => dto.type.Equals(x, StringComparison.InvariantCultureIgnoreCase)),
                question_name = dto.title.ClearName(),
                id = BlackboardHelper.GetBBId(dto.id),
                rows = dto.rows,
                answers = ParseAnswers(dto)
            };
            ret.question_text = dto.htmlText.ClearName();
            ret.htmlText = dto.htmlText;
            var imageLinks = dto.answersImageLinks as JToken;
            ret.answersImageLinks = imageLinks != null ? imageLinks.ToObject<List<string>>() : null;
            ret.answers.ForEach(
                a =>
                {
                    a.text = a.text.ClearName();
                    a.question_text = a.question_text.ClearName();
                });
            ret.caseSensitive = ret.answers.Any(x => x.caseSensitive);
            return ret;
        }

        protected virtual List<AnswerDTO> ParseAnswers(BBQuestionDTO q)
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
                    //var fileData = answersList["imageBinary"].ToString();
                    byte[] fileData = null;
                    var lazyLoadImages = images();
                    lazyLoadImages.TryGetValue(image, out fileData);

                    var answerDto = new AnswerDTO()
                    {
                        text = coords,
                        question_text = image,
                        //fileData = Encoding.UTF8.GetString(fileData)
                    };
                    if (fileData != null)
                        answerDto.fileData = Encoding.UTF8.GetString(fileData);
                    ret.Add(answerDto);
                    return ret;
                }
                // todo: this code is not needed anymore, need to check and remove. It was used for 'fill in multiple blanks' question type which has separate parser now
                var i = 0;
                foreach (var answer in answersList)
                {
                    var dto = ParseFillInBlank(answer.Value, answer.Key, i);
                    ret.Add(dto);
                    i++;
                }

                return ret;
                // end of code which should be removed
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
                    string questionText = null, answerText = null, lmsValue = null;
                    byte[] matchingImage = null;

                    if (answer is JObject)
                    {
                        foreach (var option in answer as JObject)
                        {
                            questionText = option.Key;
                            answerText = option.Value.ToString();
                            break;
                        }
                        var propIndex = (answer as JObject).Properties().FirstOrDefault(x => x.Name == "index");
                        if (propIndex != null)
                        {
                            lmsValue = propIndex.Value.ToString();
                            int propIndexInt;
                            var isIndexInt = int.TryParse(lmsValue, out propIndexInt);
                            if (isIndexInt)
                            {
                                if (propIndexInt % 2 == 0)
                                {
                                    var leftImage = (answer as JObject).Properties().FirstOrDefault(x => x.Name == "leftImageLink");
                                    if (leftImage != null)
                                    {
                                        var lazyLoadImages = images();
                                        lazyLoadImages.TryGetValue(leftImage.Value.ToString(), out matchingImage);
                                    }
                                }
                                else
                                {
                                    var rightImage = (answer as JObject).Properties().FirstOrDefault(x => x.Name == "rightImageLink");
                                    if (rightImage != null)
                                    {
                                        var lazyLoadImages = images();
                                        lazyLoadImages.TryGetValue(rightImage.Value.ToString(), out matchingImage);
                                    }
                                }
                            }

                        }

                    }
                    else
                    {
                        answerText = answer.ToString();
                        int.TryParse(answerText, out order);
                    }

                    var answerDto = new AnswerDTO()
                    {
                        id = i,
                        match_id = lmsValue,
                        text = answers != null && answers.Count > i ? answers[i] : answerText,
                        order = order,
                        question_text = questionText,
                        /*weight = i == correctAnswerId ? 100 : 0*/
                        weight = q.type.Equals("Fill in the blank", StringComparison.OrdinalIgnoreCase) ? 100 : i == correctAnswerId ? 100 : 0
                    };
                    if (matchingImage != null)
                        answerDto.fileData = Encoding.UTF8.GetString(matchingImage);
                    ret.Add(answerDto);

                    i++;
                }

                return ret;
            }

            return new List<AnswerDTO>() { new AnswerDTO() { text = "no answer", weight = 100, id = 0 } };
        }

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

        // todo: check and remove if no other types than 'fill in multiple blanks' were parsed with this code
        private static AnswerDTO ParseFillInBlank(JToken answersList, string answerText, int answerId)
        {
            bool isList = false;
            var stringValue = string.Empty;
            if (answersList is JContainer && answersList.Any())
            {
                isList = true;
                stringValue = String.Join("$$", answersList.Select(a => ((JObject)a).Properties().FirstOrDefault(x => x.Name == "text")).Where(v => v != null).Select(u => u.Value));
            }
            else if (answersList != null)
            {
                stringValue = answersList.ToString();
            }

            var result = new AnswerDTO()
            {
                id = answerId++,
                text = isList ? stringValue : answerText,
                blank_id = answerText,
                weight = stringValue.Equals("true", StringComparison.InvariantCultureIgnoreCase) ? 100 : 0
            };

            if (answersList != null)
            {
                result.question_type = String.Join("$$",
                    answersList.Select(a => ((JObject)a).Properties().FirstOrDefault(x => x.Name == "subType"))
                        .Where(v => v != null)
                        .Select(u => u.Value));
                result.caseSensitive =
                    answersList.Any(
                        a =>
                            ((JObject)a).Properties()
                                .FirstOrDefault(x => x.Name == "caseSensitive" && x.Value.ToString() == "true") != null);
            }

            return result;
        }
    }
}
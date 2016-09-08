using System;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Newtonsoft.Json.Linq;

namespace EdugameCloud.Lti.Sakai
{
    internal sealed class SakaiMultipleChoiceParser : SakaiCommonQuestionParser
    {
        private readonly string[] singleQuestionTypes = new[]
        {
            "Multiple Choice"
            // "Opinion Scale"
        };

        public override LmsQuestionDTO ParseQuestion(BBQuestionDTO dto)
        {
            var ret = new LmsQuestionDTO()
            {
                question_text = dto.text.ClearName(),
                question_type = dto.type,
                is_single = singleQuestionTypes.Any(
                    x => dto.type.Equals(x, StringComparison.InvariantCultureIgnoreCase)),
                question_name = dto.title.ClearName(),
                id = int.Parse(dto.id),
                rows = dto.rows,
                answers = ParseAnswers(dto)
            };
            ret.answers.ForEach(
                a =>
                {
                    a.text = a.question_text;
                    a.question_text = a.question_text.ClearName();
                });
            ret.caseSensitive = ret.answers.Any(x => x.caseSensitive);

            var lmsQuestion = ret;
            
            if (!string.IsNullOrEmpty(dto.questionImageBinary))
            {
                var fileDto = new LmsQuestionFileDTO
                {
                    fileName = !string.IsNullOrEmpty(dto.questionImageLink) ? dto.questionImageLink.Split('/').Last() : string.Empty,
                    fileUrl = dto.questionImageLink,
                    base64Content = dto.questionImageBinary
                };
                lmsQuestion.files.Add(0, fileDto);
            }

            return lmsQuestion;
        }

        protected override List<AnswerDTO> ParseAnswers(BBQuestionDTO q)
        {
            //if (!string.IsNullOrEmpty(q.answer))
            //{
            //    var result = q.answer.Split(';').Select(x => new AnswerDTO()
            //    {
            //        text = x,
            //        weight = 100,
            //        id = 0,
            //    });
            //    return result.ToList();
            //}
            //return base.ParseAnswers(q);

            var ret = new List<AnswerDTO>();

            //if (q.answersList is JObject)
            //{
            //    var answersList = q.answersList as JObject;

            //    if (answersList["image"] != null)
            //    {
            //        var coords = answersList["coord"].ToString();
            //        if (coords.Length < 4)
            //        {
            //            return ret;
            //        }
            //        var image = answersList["image"].ToString();
            //        var fileData = answersList["imageBinary"].ToString();

            //        ret.Add(new AnswerDTO()
            //        {
            //            text = coords,
            //            question_text = image,
            //            fileData = fileData
            //        });
            //        return ret;
            //    }
              
            //    return ret;
            //    // end of code which should be removed
            //}


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
                    string questionText = null, answerText = null, imageBinary = string.Empty, imageName = String.Empty;

                    // multiple answer
                    if (answer is JObject)
                    {
                        foreach (var option in answer as JObject)
                        {
                            questionText = option.Key;
                            answerText = option.Value.ToString();
                            break;
                        }

                        foreach (var option in answer as JObject)
                        {
                            if (option.Key == "imageBinary")
                            {
                                imageBinary = option.Value.ToString();
                            }
                            if (option.Key == "image")
                            {
                                imageName = option.Value.ToString();
                            }
                        }
                    }
                    // single answer
                    else
                    {
                        answerText = answer.ToString();
                        int.TryParse(answerText, out order);
                    }
                    bool isCorrect;
                    bool.TryParse(answerText, out isCorrect);

                    ret.Add(new AnswerDTO()
                    {
                        id = i,
                        text = answers != null && answers.Count > i ? answers[i] : answerText,
                        order = order,
                        question_text = questionText,
                        weight = isCorrect ? 100 : 0,
                        fileData = imageBinary,
                        imageName = imageName
                    });
                    i++;
                }
            }
            return ret;
        }
    }
}
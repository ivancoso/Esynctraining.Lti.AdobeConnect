namespace EdugameCloud.Core.Domain.Formats.WebEx
{
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.Core.Domain.Formats.Edugame;

    /// <summary>
    /// Represents WebEx format converter.
    /// </summary>
    public static class WebExConverter
    {
        /// <summary>
        /// The question types map.
        /// </summary>
        private static readonly Dictionary<WebExQuestionType, int> QuestionTypesMap = new Dictionary<WebExQuestionType, int>
        {
            { WebExQuestionType.Essay, 11 },
            { WebExQuestionType.FillBlanks, 4 },
            { WebExQuestionType.Instructions, 1 },
            { WebExQuestionType.SingleChoice, 1 },
            { WebExQuestionType.MultipleChoice, 1 },
            { WebExQuestionType.TrueFalse, 2 }
        };

        /// <summary>
        /// Converts WebEx pool to Edugame questions.
        /// </summary>
        /// <param name="value">WebEx pool.</param>
        /// <param name="questionTypes">Question types DTO.</param>
        /// <returns>Edugame questions.</returns>
        public static EdugameQuestions Convert(WebExPool value, List<QuestionType> questionTypes)
        {
            if (value == null)
            {
                return null;
            }

            var result = new EdugameQuestions();
            var questions = value.Questions.Select(question => Convert(question, questionTypes));
            result.Questions.AddRange(questions);

            return result;
        }

        /// <summary>
        /// Converts WebEx question to Edugame question.
        /// </summary>
        /// <param name="value">WebEx question.</param>
        /// <param name="questionTypes">List of question types DTO.</param>
        /// <returns>Edugame question.</returns>
        public static EdugameQuestion Convert(WebExQuestion value, List<QuestionType> questionTypes)
        {
            if (value == null)
            {
                return null;
            }

            var result = new EdugameQuestion
            {
                Distractors = value.Answers.Select(Convert).ToList(),
                Feedback = new EdugameQuestionFeedback
                {
                    Correct = string.Empty,
                    Incorrect = string.Empty
                },
                Instruction = string.Empty,
                Order = 0,
                Score = 0,
                Title = value.Title,
                Type = Convert(value.Type, questionTypes)
            };

            //to be multiple at least 2 distractors should be set as IsCorrect
            if (value.Type == WebExQuestionType.MultipleChoice)
            {
                if (result.Distractors.Count(x => x.IsCorrect) <= 1)
                {
                    result.Distractors.ForEach(x => x.IsCorrect = true);
                }
            }

            //in case of single choice only one should be true
            if (value.Type == WebExQuestionType.SingleChoice)
            {
                if (result.Distractors.Count(x => x.IsCorrect) == 0)
                {
                    var distractor = result.Distractors.FirstOrDefault();
                    if (distractor != null)
                    {
                        distractor.IsCorrect = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Converts WebEx answer to Edugame distractor.
        /// </summary>
        /// <param name="value">WebEx answer.</param>
        /// <returns>Edugame distractor.</returns>
        public static EdugameDistractor Convert(WebExAnswer value)
        {
            if (value == null)
            {
                return null;
            }

            var result = new EdugameDistractor
            {
                IsCorrect = value.IsCorrect,
                Order = 0,
                Title = value.Text
            };

            return result;
        }

        /// <summary>
        /// Converts WebEx question type to Edugame question type.
        /// </summary>
        /// <param name="value">WebEx question type.</param>
        /// <param name="questionTypes">List of question types DTO.</param>
        /// <returns>Edugame question type.</returns>
        public static EdugameQuestionType Convert(WebExQuestionType value, List<QuestionType> questionTypes)
        {
            var questionTypeId = QuestionTypesMap[value];
            var questionType = questionTypes.FirstOrDefault(type => type.Id == questionTypeId);
            if (questionType == null)
            {
                return null;
            }

            var result = new EdugameQuestionType
            {
                Id = questionType.Id,
                Description = questionType.Type
            };

            return result;
        }
    }
}
